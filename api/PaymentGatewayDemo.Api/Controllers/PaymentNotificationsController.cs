using Microsoft.AspNetCore.Mvc;
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
        // TODO: Verify authenticity of the notification
        var form = Request.Form;
        var transactionId = form["tr_id"];
        var transactionStatus = form["tr_status"];

        var log = "";
        foreach (var pair in form) log += pair.Key + " = " + pair.Value + "\n";
        _logger.LogInformation("Received notification: {Notification}", log);

        if (string.IsNullOrEmpty(transactionId) || string.IsNullOrEmpty(transactionStatus))
            return BadRequest(new { Result = false, Message = "Invalid notification" });

        await _paymentsService.HandleNotification(transactionId, transactionStatus);

        return Ok(new { Result = true });
    }
}