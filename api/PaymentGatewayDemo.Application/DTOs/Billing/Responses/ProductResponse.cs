using PaymentGatewayDemo.Application.TPay.Models;

namespace PaymentGatewayDemo.Application.DTOs.Billing.Responses;

public class ProductResponse
{
    public string Id { get; set; }
    public string ProductId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }

    public string PaymentStatus { get; set; }
    public bool OwnsProduct { get; set; }

    public TransactionResponse? PaymentObject { get; set; }
}