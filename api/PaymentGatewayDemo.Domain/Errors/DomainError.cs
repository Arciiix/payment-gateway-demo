namespace PaymentGatewayDemo.Domain.Errors;

public class DomainError
{
    public DomainError(string message, string details)
    {
        Message = message;
        Details = details;
    }

    public string Message { get; init; }
    public string Details { get; init; }
}