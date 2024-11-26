using PaymentGatewayDemo.Application.DTOs.Billing.Responses;
using PaymentGatewayDemo.Domain.Models;

namespace PaymentGatewayDemo.Application.Services.Products;

public interface IProductsService
{
    public Task<List<Product>> GetProducts(string userId);
    public Task UpdateProduct(Product product);
    public Task<List<ProductResponse>> GetProductsForUser(string userId, bool forceNew = false);

    // Returns the redirection URL
    public Task<string> BuyProduct(User user, string productId);
    public Task RefundProduct(string userId, string productId);

    Task<Dictionary<string, List<Billing>>> GetBillingsForProducts(string userId);
}