using FluentValidation;

namespace PaymentGatewayDemo.Application.DTOs.Auth.Requests;

public record RegisterDto(string Email, string Password);

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}