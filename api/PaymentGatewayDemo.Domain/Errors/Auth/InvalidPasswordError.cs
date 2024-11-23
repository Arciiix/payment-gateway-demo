namespace PaymentGatewayDemo.Domain.Errors.Auth;

public class InvalidPasswordError : DomainError
{
    public InvalidPasswordError() : base(
        "Invalid password",
        "The provided password is incorrect."
    )
    {
    }
}