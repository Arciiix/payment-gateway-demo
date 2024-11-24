using Refit;

namespace PaymentGatewayDemo.Infrastructure.Services.Payments;

public interface ITPayApi
{
    [Post("/oauth/auth")]
    Task<ApiResponse<AuthSuccessResponse>> Authorize([Body] AuthRequest request);

    [Post("/transactions")]
    Task<ApiResponse<TransactionResponse>> CreateTransaction([Body(true)] TransactionRequest request,
        [Header("Authorization")] string authorization);

    [Get("/transactions/{transactionId}")]
    Task<ApiResponse<TransactionResponse>> GetTransaction(string transactionId,
        [Header("Authorization")] string authorization);

    [Post("/transactions/{transactionId}/refunds")]
    Task<ApiResponse<object>> RefundTransaction(string transactionId, [Header("Authorization")] string authorization);
}