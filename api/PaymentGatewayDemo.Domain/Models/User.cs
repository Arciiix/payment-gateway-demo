using Microsoft.AspNetCore.Identity;

namespace PaymentGatewayDemo.Domain.Models;

public class User : IdentityUser
{
    public ICollection<Product> Products { get; set; } = new List<Product>();
}