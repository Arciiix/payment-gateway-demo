using System.ComponentModel.DataAnnotations;
using FluentValidation;
using PaymentGatewayDemo.Domain.Models;

namespace PaymentGatewayDemo.Application.DTOs.Auth.Requests;

public class LoginDto
{
    [Required] public string Email { get; set; }

    [Required] public string Password { get; set; }

    public static LoginDto FromRegister(RegisterDto user)
    {
        return new LoginDto
        {   
            Email = user.Email,
            Password = user.Password
        };
    }
}

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}