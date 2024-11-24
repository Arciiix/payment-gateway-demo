using System.Security.Claims;
using PaymentGatewayDemo.Application.DTOs.Auth.Requests;
using PaymentGatewayDemo.Application.DTOs.Auth.Responses;
using PaymentGatewayDemo.Domain.Errors;
using PaymentGatewayDemo.Domain.Extensions;

namespace PaymentGatewayDemo.Application.Services.Auth;

public interface IAuthService
{
    Task<Result<AuthResponse, DomainError>> Login(LoginDto request);
    Task<Result<AuthResponse, DomainError>> Register(RegisterDto request);
    Task<Result<UserResponse, DomainError>> GetUserFromRequest(ClaimsPrincipal user);
}