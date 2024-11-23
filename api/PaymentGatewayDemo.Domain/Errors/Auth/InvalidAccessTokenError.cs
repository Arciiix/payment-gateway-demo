namespace PaymentGatewayDemo.Domain.Errors.Auth;

public class InvalidAccessTokenError : DomainError
{
    public InvalidAccessTokenError() : base("Invalid access token",
        "The provided access token is invalid")
    {
    }
}