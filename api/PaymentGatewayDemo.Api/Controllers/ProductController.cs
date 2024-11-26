using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PaymentGatewayDemo.Application.DTOs.Billing.Responses;
using PaymentGatewayDemo.Application.Services.Auth;
using PaymentGatewayDemo.Application.Services.Products;
using PaymentGatewayDemo.Domain.Errors;
using PaymentGatewayDemo.Domain.Errors.Auth;
using PaymentGatewayDemo.Domain.Models;
using PaymentGatewayDemo.Infrastructure.Services.Payments;

namespace PaymentGatewayDemo.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IProductsService _productsService;


    public ProductController(PaymentsService paymentsService, IProductsService productsService,
        UserManager<User> userManager, IAuthService authService)
    {
        _productsService = productsService;
        _authService = authService;
    }


    [HttpGet]
    [Authorize]
    public async Task<List<ProductResponse>> GetProducts()
    {
        // / - returns the data that is stored in the database without any external requests to the external payment gateway API
        var userId = _authService.GetUserIdFromRequest(User);
        return await _productsService.GetProductsForUser(userId);
    }

    [HttpGet("new")]
    [Authorize]
    public async Task<List<ProductResponse>> GetProductsRefreshed()
    {
        // /new - returns the possible latest data - fetches the external payment gateway API
        var userId = _authService.GetUserIdFromRequest(User);
        return await _productsService.GetProductsForUser(userId, true);
    }

    [HttpPost("{productId}/buy")]
    [Authorize]
    public async Task<ActionResult> BuyProduct(string productId)
    {
        var user = await _authService.GetUserFromRequest(User);
        if (!user.IsOk) throw new DomainException(new UserNotFoundError());

        var url = await _productsService.BuyProduct(user.Value, productId);

        return Ok(new
        {
            Url = url
        });
    }

    [HttpPost("{productId}/refund")]
    [Authorize]
    public async Task<ActionResult> RefundProduct(string productId)
    {
        var userId = _authService.GetUserIdFromRequest(User);

        await _productsService.RefundProduct(userId, productId);

        return Ok();
    }

    [HttpGet("billings")]
    [Authorize]
    public async Task<Dictionary<string, List<Billing>>> GetBillingsForProducts()
    {
        var userId = _authService.GetUserIdFromRequest(User);

        var result = await _productsService.GetBillingsForProducts(userId);

        return result;
    }
}