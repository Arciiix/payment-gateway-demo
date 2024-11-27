using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PaymentGatewayDemo.Application.DTOs.Billing.Responses;
using PaymentGatewayDemo.Application.DTOs.Product.Requests;
using PaymentGatewayDemo.Application.Services.Products;
using PaymentGatewayDemo.Application.TPay.Models;
using PaymentGatewayDemo.Domain.Entities.Configuration;
using PaymentGatewayDemo.Domain.Errors;
using PaymentGatewayDemo.Domain.Errors.Products;
using PaymentGatewayDemo.Domain.Extensions;
using PaymentGatewayDemo.Domain.Models;
using PaymentGatewayDemo.Infrastructure.Services.Payments;
using PaymentGatewayDemo.Persistance;

namespace PaymentGatewayDemo.Infrastructure.Services.Products;

public class ProductsService : IProductsService
{
    private readonly BillingDbContext _context;
    private readonly PaymentsService _paymentsService;
    private readonly TPayConfiguration _tpayConfiguration;

    public ProductsService(BillingDbContext context, PaymentsService paymentsService,
        IOptions<GlobalConfiguration> config)
    {
        _context = context;
        _paymentsService = paymentsService;
        _tpayConfiguration = config.Value.TPayConfiguration;
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

                        var billing = await _context.Billings.FirstAsync(e => e.TransactionId == product.TransactionId);
                        billing.Status = transactionStatus.Status;
                        billing.RealizationDate = !string.IsNullOrEmpty(transactionStatus.Date.Realization)
                            ? DateTimeOffset.Parse(transactionStatus.Date.Realization)
                            : null;

                        _context.Billings.Update(billing);
                        await _context.SaveChangesAsync();

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

    public async Task<Result<string, DomainError>> RefundProduct(string userId, string productId)
    {
        var product = await _context.Products.FirstAsync(e => e.UserId == userId && e.ProductId == productId);

        if (product is null || !product.OwnsProduct) return new DoesntOwnProductError();
        var result = await _paymentsService.RefundTransaction(product.TransactionId);

        if (!result.IsOk) return result.Error;

        product.OwnsProduct = false;
        product.TransactionStatus = "refunded";

        await UpdateProduct(product);

        return "OK";
    }

    public async Task<Dictionary<string, List<Billing>>> GetBillingsForProducts(string userId)
    {
        var products = await GetProducts(userId);
        var productIds = products.Select(p => p.Id).ToList();

        var allBillings = await _context.Billings
            .Where(b => productIds.Contains(b.ProductKeyId))
            .ToListAsync();

        var billings = products.ToDictionary(
            product => product.ProductId,
            product => allBillings.Where(b => b.ProductKeyId == product.Id).Select(billing =>
            {
                billing.Product = null;
                return billing;
            }).Reverse().ToList());


        return billings;
    }

    public async Task<ProductResponse> AddProduct(AddProduct product, string userId)
    {
        var newProduct = await _context.Products.AddAsync(new Product
        {
            UserId = userId,
            ProductId = Guid.NewGuid().ToString(),
            Title = product.Title,
            Description = product.Description,
            Price = (int)Math.Floor(product.Price * 100),
            OwnsProduct = false,
            TransactionStatus = null,
            TransactionId = null
        });

        await _context.SaveChangesAsync();

        return new ProductResponse(newProduct.Entity);
    }

    public async Task<Result<string, DomainError>> BuyProduct(User user, string productId)
    {
        var product = await _context.Products.FirstAsync(e => e.UserId == user.Id && e.ProductId == productId);

        if (product is null) throw new DomainException(new ProductNotFoundError());
        if (product.OwnsProduct) throw new DomainException(new AlreadyOwnsProductError());


        var responsePayment = await _paymentsService.InitializePaymentAsync(new TransactionRequest
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
                    Email = user.Email,
                    Url = _tpayConfiguration.CallbackUrl
                },
                PayerUrls = new PayerUrls
                {
                    Success = new Uri(_tpayConfiguration.RedirectUrlSuccess),
                    Error = new Uri(_tpayConfiguration.RedirectUrlFailure)
                }
            }
        });

        if (!responsePayment.IsOk) return responsePayment.Error;
        var response = responsePayment.Value;

        product.TransactionStatus = "init";
        product.TransactionId = response.TransactionId;
        await UpdateProduct(product);

        await _context.Billings.AddAsync(
            new Billing
            {
                ProductKeyId = product.Id,
                Title = response.Title,
                FriendlyTitle = product.Title,
                Price = (decimal)product.Price / 100,
                UserId = user.Id,
                CreationDate = DateTimeOffset.Now,
                TransactionId = response.TransactionId,
                Status = "init"
            });

        await _context.SaveChangesAsync();

        return response.TransactionPaymentUrl;
    }
}