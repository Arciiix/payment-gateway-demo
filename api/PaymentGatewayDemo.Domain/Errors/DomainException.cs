namespace PaymentGatewayDemo.Domain.Errors;

public class DomainException : Exception
{
    public DomainException(DomainError error) : base(error.Message)
    {
    }
}