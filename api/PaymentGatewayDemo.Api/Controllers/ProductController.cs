using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentGatewayDemo.Application.DTOs.Billing.Responses;
using PaymentGatewayDemo.Application.DTOs.Product.Requests;
using PaymentGatewayDemo.Application.Services.Auth;
using PaymentGatewayDemo.Application.Services.Products;
using PaymentGatewayDemo.Domain.Errors;
using PaymentGatewayDemo.Domain.Errors.Auth;
using PaymentGatewayDemo.Domain.Models;

namespace PaymentGatewayDemo.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IProductsService _productsService;


    public ProductController(IProductsService productsService, IAuthService authService)
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

    [HttpPost]
    [Authorize]
    public async Task<ProductResponse> AddProduct(AddProduct product)
    {
        var userId = _authService.GetUserIdFromRequest(User);
        return await _productsService.AddProduct(product, userId);
    }

    [HttpPost("{productId}/buy")]
    [Authorize]
    public async Task<ActionResult> BuyProduct(string productId)
    {
        var user = await _authService.GetUserFromRequest(User);
        if (!user.IsOk) throw new DomainException(new UserNotFoundError());

        var response = await _productsService.BuyProduct(user.Value, productId);

        return response.Match(
            success => Ok(new { Url = success }),
            failure => new HttpErrorResponse(StatusCodes.Status500InternalServerError, failure).ToActionResult());
    }

    [HttpPost("{productId}/refund")]
    [Authorize]
    public async Task<ActionResult> RefundProduct(string productId)
    {
        var userId = _authService.GetUserIdFromRequest(User);

        var result = await _productsService.RefundProduct(userId, productId);

        return result.Match(
            success => Ok(),
            failure => new HttpErrorResponse(StatusCodes.Status500InternalServerError, failure).ToActionResult());
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