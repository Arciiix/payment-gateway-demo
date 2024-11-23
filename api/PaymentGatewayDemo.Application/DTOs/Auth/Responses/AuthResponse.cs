namespace PaymentGatewayDemo.Application.DTOs.Auth.Responses;

public class AuthResponse
{
    public required string Email { get; set; }
    public required string AccessToken { get; set; }
    public DateTime AccessTokenExpiryTime { get; set; }
}