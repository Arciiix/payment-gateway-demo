using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PaymentGatewayDemo.Api.Helpers;
using PaymentGatewayDemo.Application.DTOs.Auth.Requests;
using PaymentGatewayDemo.Application.DTOs.Auth.Responses;
using PaymentGatewayDemo.Application.Services.Auth;
using PaymentGatewayDemo.Domain.Entities.Configuration;
using PaymentGatewayDemo.Domain.Errors.Auth;
using Swashbuckle.AspNetCore.Annotations;

namespace PaymentGatewayDemo.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IOptions<GlobalConfiguration> _config;

    public AuthController(IAuthService authService, IOptions<GlobalConfiguration> config)
    {
        _authService = authService;
        _config = config;
    }

    [SwaggerResponse(StatusCodes.Status200OK, "User logged in", typeof(AuthResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid password", typeof(HttpErrorResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found", typeof(HttpErrorResponse))]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginDto request)
    {
        var response = await _authService.Login(request);

        return response.Match(
            success =>
            {
                SetTokenCookie(success.AccessToken);
                return Ok(success);
            },
            failure =>
            {
                var code = StatusCodes.Status500InternalServerError;

                if (failure is InvalidPasswordError)
                    code = StatusCodes.Status401Unauthorized;
                else if (failure is UserNotFoundError)
                    code = StatusCodes.Status404NotFound;

                return new HttpErrorResponse(code, failure).ToActionResult();
            }
        );
    }

    [HttpPost("register")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "User already exists", typeof(HttpErrorResponse))]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterDto request)
    {
        var response = await _authService.Register(request);
        return response.Match(
            success =>
            {
                SetTokenCookie(success.AccessToken);
                return Ok(success);
            },
            failure =>
            {
                if (failure is UserAlreadyExistsError)
                    return new HttpErrorResponse(StatusCodes.Status409Conflict, failure).ToActionResult();
                return new HttpErrorResponse(StatusCodes.Status500InternalServerError, failure).ToActionResult();
            }
        );
    }

    [HttpDelete("logout")]
    [Authorize]
    public ActionResult Logout()
    {
        Response.Cookies.Delete(AuthHelpers.AccessTokenCookieName);
        return Ok();
    }

    [HttpGet("user")]
    [Authorize]
    public async Task<ActionResult<UserResponse>> GetUser()
    {
        var response = await _authService.GetUserFromRequest(User);

        return response.Match(
            e => Ok(new UserResponse
            {
                Email = e.Email
            }),
            failure => new HttpErrorResponse(StatusCodes.Status500InternalServerError, failure).ToActionResult());
    }


    private void SetTokenCookie(string accessToken)
    {
        var accessCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,

            Expires = DateTime.UtcNow.AddMinutes(_config.Value.JwtConfiguration.TokenValidityInMinutes)
        };
        Response.Cookies.Append(AuthHelpers.AccessTokenCookieName, accessToken, accessCookieOptions);
    }
}