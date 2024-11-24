using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PaymentGatewayDemo.Application.DTOs.Auth.Requests;
using PaymentGatewayDemo.Application.DTOs.Auth.Responses;
using PaymentGatewayDemo.Application.Services.Auth;
using PaymentGatewayDemo.Domain.Entities.Configuration;
using PaymentGatewayDemo.Domain.Errors;
using PaymentGatewayDemo.Domain.Errors.Auth;
using PaymentGatewayDemo.Domain.Extensions;
using PaymentGatewayDemo.Domain.Models;
using PaymentGatewayDemo.Persistance;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace PaymentGatewayDemo.Infrastructure.Services.Auth;

public class AuthService : IAuthService
{
    private readonly BillingDbContext _db;
    private readonly ILogger<AuthService> _logger;
    private readonly IOptions<GlobalConfiguration> _options;
    private readonly UserManager<User> _userManager;

    public AuthService(BillingDbContext db, ILogger<AuthService> logger, IOptions<GlobalConfiguration> options,
        UserManager<User> userManager)
    {
        _db = db;
        _logger = logger;
        _options = options;
        _userManager = userManager;
    }

    public async Task<Result<AuthResponse, DomainError>> Login(LoginDto request)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(x =>
            x.NormalizedEmail.ToLower() == request.Email.ToLower());
        if (user == null) return new UserNotFoundError();

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
        {
            await _userManager.AccessFailedAsync(user);
            return new InvalidPasswordError();
        }

        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in await _userManager.GetRolesAsync(user)) authClaims.Add(new Claim(ClaimTypes.Role, role));

        var token = CreateToken(authClaims);

        await _userManager.UpdateAsync(user);
        await _db.SaveChangesAsync();

        _logger.LogInformation("User logged in: {Email}", user.Email);

        return new AuthResponse
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            AccessTokenExpiryTime = token.ValidTo,
            Email = user.Email
        };
    }

    public async Task<Result<AuthResponse, DomainError>> Register(RegisterDto request)
    {
        var userExists = await _userManager.FindByEmailAsync(request.Email);
        if (userExists is not null) return new UserAlreadyExistsError();


        var user = new User
        {
            Email = request.Email,
            UserName = request.Email,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            _logger.LogError("User creation failed: {Errors}", result.Errors);
            throw new Exception(result.Errors.First().Description);
        }

        _logger.LogInformation("User created: {Email}", user.Email);

        return await Login(LoginDto.FromRegister(request));
    }

    public async Task<Result<UserResponse, DomainError>> GetUserFromRequest(ClaimsPrincipal userClaims)
    {
        var email = userClaims.FindFirstValue(ClaimTypes.Email);

        if (email is null) throw new DomainException(new UserNotFoundError());

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null) return new UserNotFoundError();

        return new UserResponse
        {
            Email = user.Email
        };
    }

    private JwtSecurityToken CreateToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.JwtConfiguration.Secret));


        var token = new JwtSecurityToken(
            _options.Value.JwtConfiguration.Issuer,
            _options.Value.JwtConfiguration.Audience,
            expires: DateTime.UtcNow.AddMinutes(_options.Value.JwtConfiguration.TokenValidityInMinutes),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        _logger.LogInformation("Token created for {Email}",
            authClaims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value ?? "?");

        return token;
    }
}