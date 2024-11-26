using Microsoft.EntityFrameworkCore;
using PaymentGatewayDemo.Application.DTOs.Billing.Responses;
using PaymentGatewayDemo.Application.Services.Products;
using PaymentGatewayDemo.Application.TPay.Models;
using PaymentGatewayDemo.Domain.Errors;
using PaymentGatewayDemo.Domain.Errors.Products;
using PaymentGatewayDemo.Domain.Models;
using PaymentGatewayDemo.Infrastructure.Services.Payments;
using PaymentGatewayDemo.Persistance;

namespace PaymentGatewayDemo.Infrastructure.Services.Products;

public class ProductsService : IProductsService
{
    private readonly BillingDbContext _context;
    private readonly PaymentsService _paymentsService;

    public ProductsService(BillingDbContext context, PaymentsService paymentsService)
    {
        _context = context;
        _paymentsService = paymentsService;
    }

    public Task<List<Product>> GetProducts(string userId)
    {
        return _context.Products.Where(e => e.UserId == userId).ToListAsync();
    }

    public async Task UpdateProduct(Product product)
    {
        _context.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ProductResponse>> GetProductsForUser(string userId, bool forceNew = false)
    {
        var products = await GetProducts(userId);
        var productResponses = new List<ProductResponse>();

        // Fetch the corresponding transaction data for all the products and update its status if needed
        foreach (var product in products)
            if (product.TransactionId != null)
            {
                if (forceNew)
                {
                    var transactionStatus = await _paymentsService.GetTransactionStatus(product.TransactionId);

                    if ((transactionStatus?.Status is not null &&
                         transactionStatus.Status != product.TransactionStatus) ||
                        (!product.OwnsProduct && transactionStatus.Status == "correct") ||
                        (product.OwnsProduct && transactionStatus.Status != "correct"))
                    {
                        product.TransactionStatus = transactionStatus.Status;
                        product.OwnsProduct = transactionStatus.Status == "correct";
                        await UpdateProduct(product);
                    }

                    productResponses.Add(new ProductResponse
                    {
                        Id = product.Id.ToString(),
                        ProductId = product.ProductId,
                        Title = product.Title,
                        Description = product.Description,
                        Price = product.Price,
                        OwnsProduct = product.TransactionStatus == "correct",
                        PaymentObject = transactionStatus,
                        PaymentStatus = transactionStatus?.Status ?? "unknown"
                    });
                }
                else
                {
                    productResponses.Add(new ProductResponse
                    {
                        Id = product.Id.ToString(),
                        ProductId = product.ProductId,
                        Title = product.Title,
                        Description = product.Description,
                        Price = product.Price,
                        OwnsProduct = product.TransactionStatus == "correct",
                        PaymentObject = null,
                        PaymentStatus = product.TransactionStatus ?? "unknown"
                    });
                }
            }
            else
            {
                productResponses.Add(new ProductResponse
                {
                    Id = product.Id.ToString(),
                    ProductId = product.ProductId,
                    Title = product.Title,
                    Description = product.Description,
                    Price = product.Price,
                    OwnsProduct = false,
                    PaymentObject = null,
                    PaymentStatus = "no payment"
                });
            }

        return productResponses;
    }

    public async Task RefundProduct(string userId, string productId)
    {
        var product = await _context.Products.FirstAsync(e => e.UserId == userId && e.ProductId == productId);

        if (product is null || !product.OwnsProduct) throw new DomainException(new DoesntOwnProductError());
        await _paymentsService.RefundTransactionAsync(product.TransactionId);
        product.OwnsProduct = false;
        product.TransactionStatus = "refunded";

        await UpdateProduct(product);
    }

    public async Task<string> BuyProduct(User user, string productId)
    {
        var product = await _context.Products.FirstAsync(e => e.UserId == user.Id && e.ProductId == productId);

        if (product is null) throw new DomainException(new ProductNotFoundError());
        if (product.OwnsProduct) throw new DomainException(new AlreadyOwnsProductError());


        var response = await _paymentsService.InitializePaymentAsync(new TransactionRequest
        {
            Amount = (decimal)product.Price / 100,
            Description = product.Title,
            Payer = new PayerInfo
            {
                Email = user.Email,
                Name = user.Email
            },
            Callbacks = new Callbacks
            {
                Notification = new Notification
                {
                    Email = user.Email
                },
                PayerUrls = new PayerUrls
                {
                    // TODO: DEV
                    Success = new Uri("http://localhost:5238/payment/success"),
                    Error = new Uri("http://localhost:5238/payment/error")
                }
            }
        });

        product.TransactionStatus = "init";
        product.TransactionId = response.TransactionId;
        await UpdateProduct(product);

        return response.TransactionPaymentUrl;
    }
}