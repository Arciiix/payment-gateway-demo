namespace PaymentGatewayDemo.Domain.Errors.Products;

public class AlreadyOwnsProductError : DomainError
{
    public AlreadyOwnsProductError() : base("You already own the product", "You can't buy it again")
    {
    }
}