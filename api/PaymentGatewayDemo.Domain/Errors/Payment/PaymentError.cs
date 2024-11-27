namespace PaymentGatewayDemo.Domain.Errors.Payment;

public class PaymentError : DomainError
{
    public PaymentError() : base(
        "Payment error",
        "An error occurred while processing the payment."
    )
    {
    }
}