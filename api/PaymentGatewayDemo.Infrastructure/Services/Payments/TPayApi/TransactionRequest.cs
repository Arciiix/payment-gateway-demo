using Newtonsoft.Json;

namespace PaymentGatewayDemo.Infrastructure.Services.Payments;

public class TransactionRequest
{
    [JsonProperty("amount")] public decimal Amount { get; set; }

    [JsonProperty("description")] public string Description { get; set; }

    [JsonProperty("payer")] public PayerInfo Payer { get; set; }

    [JsonProperty("callbacks")] public Callbacks Callbacks { get; set; }
}

public class Callbacks
{
    [JsonProperty("payerUrls")] public PayerUrls PayerUrls { get; set; }

    [JsonProperty("notification")] public Notification Notification { get; set; }
}

public class PayerUrls
{
    [JsonProperty("success")] public Uri Success { get; set; }
    [JsonProperty("error")] public Uri Error { get; set; }
}

public class Notification
{
    [JsonProperty("url")] public string Url { get; set; }
    [JsonProperty("email")] public string Email { get; set; }
}

public class PayerInfo
{
    [JsonProperty("email")] public string Email { get; set; }

    [JsonProperty("name")] public string Name { get; set; }
}