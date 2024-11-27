using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PaymentGatewayDemo.Infrastructure.Helpers;
using PaymentGatewayDemo.Infrastructure.Services.Payments;

namespace PaymentGatewayDemo.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentNotificationsController : ControllerBase
{
    private readonly ILogger<PaymentNotificationsController> _logger;
    private readonly PaymentsService _paymentsService;

    public PaymentNotificationsController(ILogger<PaymentNotificationsController> logger,
        PaymentsService paymentsService)
    {
        _logger = logger;
        _paymentsService = paymentsService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return Ok("Payment Gateway Demo - Notification Receiver");
    }

    [HttpPost]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> ProcessNotification()
    {
        // Based on https://docs.tpay.com/#!/Tpay/tpay_notifications 

        // Get JWS signature header
        var jws = Request.Headers["X-JWS-Signature"].FirstOrDefault();
        if (string.IsNullOrEmpty(jws)) return ReturnBadRequest("Missing JWS header");

        // Split JWS into components
        var jwsParts = jws.Split('.');
        if (jwsParts.Length != 3) return ReturnBadRequest("Invalid JWS header");

        var headers = jwsParts[0];
        var signature = jwsParts[2];

        // Decode headers from Base64 URL-safe format
        var headersJson = Encoding.UTF8.GetString(TPayNotificationValidationHelper.Base64UrlDecode(headers));

        // Parse headers JSON
        var headersData = JsonSerializer.Deserialize<JsonElement>(headersJson);
        if (!headersData.TryGetProperty("x5u", out var x5uElement)) return ReturnBadRequest("Missing x5u header");

        var x5u = x5uElement.GetString();
        if (string.IsNullOrEmpty(x5u) || !x5u.StartsWith("https://secure.sandbox.tpay.com"))
            return ReturnBadRequest("Wrong x5u URL");


        // Fetch JWS signing certificate
        string certificate;
        try
        {
            using var httpClient = new HttpClient();
            certificate = await httpClient.GetStringAsync(x5u);
        }
        catch
        {
            return ReturnBadRequest("Unable to fetch signing certificate");
        }

        // Fetch CA certificate
        string caCertificate;
        try
        {
            using var httpClient = new HttpClient();
            caCertificate =
                await httpClient.GetStringAsync("https://secure.sandbox.tpay.com/x509/tpay-jws-root.pem");
        }
        catch
        {
            return ReturnBadRequest("Unable to fetch Tpay CA certificate");
        }

        // Verify signing certificate with CA
        if (!TPayNotificationValidationHelper.VerifyCertificate(certificate, caCertificate))
            return ReturnBadRequest("Signing certificate is not signed by Tpay CA certificate");

        using var reader = new StreamReader(Request.Body);
        reader.BaseStream.Seek(0, SeekOrigin.Begin);
        var body = await reader.ReadToEndAsync();

        // Encode body to Base64 URL-safe format
        var payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(body))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');

        // Decode received signature
        var decodedSignature = TPayNotificationValidationHelper.Base64UrlDecode(signature);

        // Verify the JWS signature
        if (!TPayNotificationValidationHelper.VerifySignature(headers + "." + payload, decodedSignature, certificate))
            return ReturnBadRequest("Invalid JWS signature");

        // Seek back to the beginning of the stream
        Request.Body.Seek(0, SeekOrigin.Begin);

        var form = await Request.ReadFormAsync();
        var transactionId = form["tr_id"];
        var transactionStatus = form["tr_status"];

        var log = "";
        foreach (var pair in form) log += pair.Key + " = " + pair.Value + "\n";
        _logger.LogInformation("Received notification: {Notification}", log);

        if (string.IsNullOrEmpty(transactionId) || string.IsNullOrEmpty(transactionStatus))
            return ReturnBadRequest("Missing transaction ID or status");

        await _paymentsService.HandleNotification(transactionId, transactionStatus);

        return Ok(new { Result = true });
    }


    private ActionResult ReturnBadRequest(string message)
    {
        _logger.LogError(message);
        return BadRequest(new { Result = false, Message = message });
    }
}