using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentGatewayDemo.Domain.Entities.Configuration;

namespace PaymentGatewayDemo.Infrastructure.Services.Payments;

public class PaymentsService
{
    private readonly ITPayApi _api;
    private readonly TPayConfiguration _configuration;
    private readonly ILogger<PaymentsService> _logger;

    public TToken? Token;

    public PaymentsService(ITPayApi api, IOptions<GlobalConfiguration> config, ILogger<PaymentsService> logger)
    {
        _api = api;
        _logger = logger;
        _configuration = config.Value.TPayConfiguration;
    }

    public async Task<TransactionResponse?> InitializePaymentAsync(TransactionRequest request)
    {
        var token = await GetAuthorization();
        var response = await _api.CreateTransaction(request, token).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Payment service failed to initialize a new transaction: {StatusCode}: {ErrorResponse}",
                response.StatusCode, response.Content);
            return null;
        }

        return response.Content;
    }

    public async Task<TransactionResponse?> GetTransactionStatus(string transactionId)
    {
        var response = await _api.GetTransaction(transactionId, await GetAuthorization()).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to get transaction status: {StatusCode}", response.StatusCode);
            return null;
        }

        return response.Content;
    }

    private async Task<string> GetAuthorization()
    {
        if (Token != null && Token.expires > DateTime.Now) return "Bearer " + Token.token;

        var authorized = await Authorize();
        return "Bearer " + authorized.token;
    }

    private async Task<TToken> Authorize()
    {
        var response = await _api.Authorize(
            new AuthRequest
            {
                ClientId = _configuration.ClientId,
                ClientSecret = _configuration.ClientSecret
            });


        if (response.StatusCode != HttpStatusCode.OK
            || response.Content?.AccessToken is null)
        {
            _logger.LogError($"Authorization failed: {response.StatusCode}");
            throw new Exception($"Authorization failed: {response.StatusCode}");
        }


        Token = new TToken(response.Content.AccessToken, DateTime.Now.AddSeconds(response.Content.ExpiresIn));
        return Token;
    }

    public record TToken(string token, DateTime expires);
}