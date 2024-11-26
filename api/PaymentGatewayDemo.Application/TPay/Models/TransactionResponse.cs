using Newtonsoft.Json;

namespace PaymentGatewayDemo.Application.TPay.Models;

public class TransactionResponse
{
    [JsonProperty("result")] public string Result { get; set; }

    [JsonProperty("requestId")] public string RequestId { get; set; }

    [JsonProperty("transactionId")] public string TransactionId { get; set; }

    [JsonProperty("title")] public string Title { get; set; }

    [JsonProperty("posId")] public string PosId { get; set; }

    [JsonProperty("status")] public string Status { get; set; }

    [JsonProperty("date")] public TransactionDate Date { get; set; }

    [JsonProperty("amount")] public decimal Amount { get; set; }

    [JsonProperty("currency")] public string Currency { get; set; }

    [JsonProperty("description")] public string Description { get; set; }

    [JsonProperty("hiddenDescription")] public string HiddenDescription { get; set; }

    [JsonProperty("payer")] public Payer Payer { get; set; }

    [JsonProperty("payments")] public PaymentDetails Payments { get; set; }

    [JsonProperty("transactionPaymentUrl")]
    public string TransactionPaymentUrl { get; set; }
}

public class TransactionDate
{
    [JsonProperty("creation")] public string Creation { get; set; }

    [JsonProperty("realization")] public string Realization { get; set; }
}

public class Payer
{
    [JsonProperty("payerId")] public string PayerId { get; set; }

    [JsonProperty("email")] public string Email { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("phone")] public string Phone { get; set; }

    [JsonProperty("address")] public string Address { get; set; }

    [JsonProperty("city")] public string City { get; set; }

    [JsonProperty("country")] public string Country { get; set; }

    [JsonProperty("postalCode")] public string PostalCode { get; set; }
}

public class PaymentDetails
{
    [JsonProperty("status")] public string Status { get; set; }

    [JsonProperty("method")] public string Method { get; set; }

    [JsonProperty("amountPaid")] public decimal AmountPaid { get; set; }

    [JsonProperty("date")] public PaymentDate Date { get; set; }
}

public class PaymentDate
{
    [JsonProperty("realization")] public string Realization { get; set; }
}