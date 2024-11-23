namespace PaymentGatewayDemo.Domain.Errors.Auth;

public class UserAlreadyExistsError : DomainError
{
    public UserAlreadyExistsError() : base(
        "User already exists",
        "The user with the provided email already exists in the system."
    )
    {
    }
}