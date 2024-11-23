using FluentValidation;

namespace PaymentGatewayDemo.Domain.Entities.Configuration;

public class JwtConfiguration
{
    public string Secret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int TokenValidityInMinutes { get; set; } = 15;
}

public class JwtConfigurationValidator : AbstractValidator<JwtConfiguration>
{
    public JwtConfigurationValidator()
    {
        RuleFor(x => x.Secret).NotEmpty();
        RuleFor(x => x.Issuer).NotEmpty();
        RuleFor(x => x.Audience).NotEmpty();
        RuleFor(x => x.TokenValidityInMinutes).GreaterThan(0);
    }
}