namespace PaymentGatewayDemo.Domain.Errors.Auth;

public class UserNotFoundError : DomainError
{
    public UserNotFoundError() : base(
        "User not found",
        "The user with the provided email does not exist in the system."
    )
    {
    }
}