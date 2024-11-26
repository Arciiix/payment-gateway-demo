using Newtonsoft.Json;

namespace PaymentGatewayDemo.Application.TPay.Models;

public class AuthSuccessResponse
{
    [JsonProperty("issued_at")] public long IssuedAt { get; set; }

    [JsonProperty("scope")] public string Scope { get; set; }

    [JsonProperty("token_type")] public string TokenType { get; set; }

    [JsonProperty("expires_in")] public int ExpiresIn { get; set; }

    [JsonProperty("client_id")] public string ClientId { get; set; }

    [JsonProperty("access_token")] public string AccessToken { get; set; }
}