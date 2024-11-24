namespace PaymentGatewayDemo.Domain.Errors.Products;

public class DoesntOwnProductError : DomainError
{
    public DoesntOwnProductError() : base("You don't own it",
        "You don't own this product")
    {
    }
}