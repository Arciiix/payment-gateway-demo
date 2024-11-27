using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentGatewayDemo.Application.TPay.Models;
using PaymentGatewayDemo.Domain.Entities.Configuration;
using PaymentGatewayDemo.Domain.Errors;
using PaymentGatewayDemo.Domain.Errors.Payment;
using PaymentGatewayDemo.Domain.Extensions;
using PaymentGatewayDemo.Persistance;

namespace PaymentGatewayDemo.Infrastructure.Services.Payments;

public class PaymentsService
{
    private readonly ITPayApi _api;
    private readonly TPayConfiguration _configuration;
    private readonly ILogger<PaymentsService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    private TToken? token;

    public PaymentsService(ITPayApi api, IOptions<GlobalConfiguration> config, IServiceScopeFactory scopeFactory,
        ILogger<PaymentsService> logger)
    {
        _api = api;
        _logger = logger;
        _scopeFactory = scopeFactory;
        _configuration = config.Value.TPayConfiguration;
    }

    public async Task<Result<TransactionResponse?, DomainError>> InitializePaymentAsync(TransactionRequest request)
    {
        var token = await GetAuthorization();
        var response = await _api.CreateTransaction(request, token).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Payment service failed to initialize a new transaction: {StatusCode}: {ErrorResponse}",
                response.StatusCode, response.Content);
            return new PaymentError();
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

    public async Task<Result<string, DomainError>> RefundTransaction(string transactionId)
    {
        var response = await _api.RefundTransaction(transactionId, await GetAuthorization()).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to refund transaction: {StatusCode} - {Content}", response.StatusCode,
                response.Content);
            return new PaymentError();
        }

        return "OK";
    }

    private async Task<string> GetAuthorization()
    {
        if (token != null && token.expires > DateTime.Now) return "Bearer " + token.token;

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


        token = new TToken(response.Content.AccessToken, DateTime.Now.AddSeconds(response.Content.ExpiresIn));
        return token;
    }

    public async Task HandleNotification(string transactionId, string status)
    {
        // We receive the notification from the external payment gateway

        // Here for some reason the transactionId is more likely to be transaction title
        // TODO: Investigate why

        await using var scope = _scopeFactory.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<BillingDbContext>();

        var billing =
            await context.Billings.FirstOrDefaultAsync(
                e => e.TransactionId == transactionId || e.Title == transactionId);

        if (billing is null)
        {
            _logger.LogWarning("Received notification for unknown transaction: {TransactionId}", transactionId);
            return;
        }

        billing.Status = status switch
        {
            "PAID" or "TRUE" => "correct",
            "CHARGEBACK" => "refunded",
            _ => status
        };

        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            context.Billings.Update(billing);
            var product = context.Products.FirstOrDefault(e => e.TransactionId == billing.TransactionId);

            if (product != null)
            {
                product.TransactionStatus = billing.Status;
                product.OwnsProduct = billing.Status == "correct";
                context.Products.Update(product);
            }
            else
            {
                _logger.LogWarning("No product found for transaction: {TransactionId}", transactionId);
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Updated transaction status for transaction: {TransactionId}", transactionId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to update transaction status");
            await transaction.RollbackAsync();
        }
    }

    public record TToken(string token, DateTime expires);
}