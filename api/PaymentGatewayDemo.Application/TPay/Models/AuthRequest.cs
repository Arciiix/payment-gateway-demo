using Newtonsoft.Json;

namespace PaymentGatewayDemo.Application.TPay.Models;

public class AuthRequest
{
    [JsonProperty("client_id")] public string ClientId { get; set; }

    [JsonProperty("client_secret")] public string ClientSecret { get; set; }
}