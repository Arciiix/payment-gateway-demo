using System.Security.Claims;
using PaymentGatewayDemo.Application.DTOs.Auth.Requests;
using PaymentGatewayDemo.Application.DTOs.Auth.Responses;
using PaymentGatewayDemo.Domain.Errors;
using PaymentGatewayDemo.Domain.Extensions;
using PaymentGatewayDemo.Domain.Models;

namespace PaymentGatewayDemo.Application.Services.Auth;

public interface IAuthService
{
    Task<Result<AuthResponse, DomainError>> Login(LoginDto request);
    Task<Result<AuthResponse, DomainError>> Register(RegisterDto request);
    Task<Result<User, DomainError>> GetUserFromRequest(ClaimsPrincipal user);
    string GetUserIdFromRequest(ClaimsPrincipal user);
}