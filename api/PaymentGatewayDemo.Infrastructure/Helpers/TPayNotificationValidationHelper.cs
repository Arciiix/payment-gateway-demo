using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace PaymentGatewayDemo.Infrastructure.Helpers;

public static class TPayNotificationValidationHelper
{
    public static bool VerifyCertificate(string certificatePem, string caCertificatePem)
    {
        using (var cert = new X509Certificate2(Encoding.UTF8.GetBytes(certificatePem)))
        using (var caCert = new X509Certificate2(Encoding.UTF8.GetBytes(caCertificatePem)))
        {
            // Ensure the signing certificate is signed by the CA certificate
            using var chain = new X509Chain();
            chain.ChainPolicy.ExtraStore.Add(caCert);
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

            return chain.Build(cert);
        }
    }

    public static bool VerifySignature(string data, byte[] signature, string certificatePem)
    {
        using (var cert = new X509Certificate2(Encoding.UTF8.GetBytes(certificatePem)))
        {
            using var publicKey = cert.GetRSAPublicKey();
            return publicKey.VerifyData(
                Encoding.UTF8.GetBytes(data),
                signature,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
            );
        }
    }

    public static byte[] Base64UrlDecode(string input)
    {
        var output = input.Replace('-', '+').Replace('_', '/');
        switch (output.Length % 4)
        {
            case 2:
                output += "==";
                break;
            case 3:
                output += "=";
                break;
        }

        return Convert.FromBase64String(output);
    }
}