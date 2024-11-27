using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace PaymentGatewayDemo.Domain.Entities.Configuration;

public class TPayConfiguration
{
    [Required] public required string ClientId { get; set; }

    [Required] public required string ClientSecret { get; set; }

    [Required] public required string CallbackUrl { get; set; }

    [Required] public required string RedirectUrlSuccess { get; set; }

    [Required] public required string RedirectUrlFailure { get; set; }
}

public class TPayConfigurationValidator : AbstractValidator<TPayConfiguration>
{
    public TPayConfigurationValidator()
    {
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.ClientSecret).NotEmpty();
        RuleFor(x => x.CallbackUrl).NotEmpty();
        RuleFor(x => x.RedirectUrlSuccess).NotEmpty();
        RuleFor(x => x.RedirectUrlFailure).NotEmpty();
    }
}