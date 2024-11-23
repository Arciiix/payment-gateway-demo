namespace PaymentGatewayDemo.Api.Helpers;

public static class AuthHelpers
{
    public static readonly string AccessTokenCookieName = "DemoAuthCookie";

    public static string? GetAccessToken(HttpRequest request)
    {
        var accessToken = request.Headers["Authorization"];

        if (string.IsNullOrEmpty(accessToken)) accessToken = request.Cookies[AccessTokenCookieName];

        return accessToken;
    }
}