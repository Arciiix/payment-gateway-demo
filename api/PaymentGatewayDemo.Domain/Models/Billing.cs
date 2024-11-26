using System.ComponentModel.DataAnnotations;

namespace PaymentGatewayDemo.Domain.Models;

public class Billing
{
    [Key] public string TransactionId { get; set; }

    public Guid ProductKeyId { get; set; }
    public Product? Product { get; set; } = null;

    public string Title { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; }

    public string UserId { get; set; }
    public User User { get; set; }

    public DateTimeOffset CreationDate { get; set; }
    public DateTimeOffset? RealizationDate { get; set; }
}