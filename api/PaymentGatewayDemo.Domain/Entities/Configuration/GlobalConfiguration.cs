using FluentValidation;

namespace PaymentGatewayDemo.Domain.Entities.Configuration;

public class GlobalConfiguration
{
    public const string SectionName = "Config";

    public required JwtConfiguration JwtConfiguration { get; set; }
    public required TPayConfiguration TPayConfiguration { get; set; }
}

public class GlobalConfigurationValidator : AbstractValidator<GlobalConfiguration>
{
    public GlobalConfigurationValidator()
    {
        RuleFor(x => x.JwtConfiguration).NotEmpty().SetValidator(new JwtConfigurationValidator());
        RuleFor(x => x.TPayConfiguration).NotEmpty().SetValidator(new TPayConfigurationValidator());
    }
}