using Microsoft.AspNetCore.Mvc;
using PaymentGatewayDemo.Infrastructure.Services.Payments;

namespace PaymentGatewayDemo.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BillingController
{
    private readonly PaymentsService _paymentsService;

    public BillingController(PaymentsService paymentsService)
    {
        _paymentsService = paymentsService;
    }

    // [HttpPost]
    // public async Task<TransactionResponse> CreateTransaction()
    // {
    //     return await _paymentsService.InitializePaymentAsync(new TransactionRequest
    //     {
    //         Amount = 100,
    //         Description = "Test",
    //         Payer = new PayerInfo
    //         {
    //             Email = "test@test.com",
    //             Name = "Test"
    //         },
    //         Callbacks = new Callbacks
    //         {
    //             Notification = new Notification
    //             {
    //                 Email = "test@test.com"
    //             },
    //             PayerUrls = new PayerUrls
    //             {
    //                 Success = new Uri("http://localhost:5238/payment/success"),
    //                 Error = new Uri("http://localhost:5238/payment/error")
    //             }
    //         }
    //     }) ?? throw new Exception();
    // }
    //
    // [HttpGet("{transactionId}")]
    // public async Task<TransactionResponse> GetTransaction(string transactionId)
    // {
    //     return await _paymentsService.GetTransactionStatus(transactionId);
    // }
}