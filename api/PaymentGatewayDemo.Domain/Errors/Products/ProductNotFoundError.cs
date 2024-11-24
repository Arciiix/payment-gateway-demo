namespace PaymentGatewayDemo.Domain.Errors.Products;

public class ProductNotFoundError : DomainError
{
    public ProductNotFoundError() : base("Not found", "Product not found")
    {
    }
}