using System.ComponentModel.DataAnnotations;

namespace PaymentGatewayDemo.Domain.Models;

public class Product
{
    public ICollection<Billing>? Billings;

    [Key] public Guid Id { get; set; }

    public string ProductId { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }

    public User User { get; set; }
    public string UserId { get; set; }

    public bool OwnsProduct { get; set; } = false;


    public string? TransactionId { get; set; }
    public string? TransactionStatus { get; set; }
}