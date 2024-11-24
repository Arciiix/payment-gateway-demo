using System.Text.Json;
using Newtonsoft.Json;

namespace PaymentGatewayDemo.Infrastructure.Services.Payments;

public class AuthRequest
{
    [JsonProperty("client_id")]
    public string ClientId { get; set; }    
    
    [JsonProperty("client_secret")]
    public string ClientSecret { get; set; }
}