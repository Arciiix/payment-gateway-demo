using System.ComponentModel.DataAnnotations;
using System.Data;
using FluentValidation;

namespace PaymentGatewayDemo.Domain.Entities.Configuration;

public class TPayConfiguration
{
    [Required]
    public required string ClientId { get; set; }
    
    [Required]
    public required string ClientSecret { get; set; }
}

public class TPayConfigurationValidator : AbstractValidator<TPayConfiguration>
{
    public TPayConfigurationValidator()
    {
       RuleFor(x => x.ClientId).NotEmpty(); 
       RuleFor(x => x.ClientSecret).NotEmpty(); 
    }
}