using FluentValidation;

namespace PaymentGatewayDemo.Application.DTOs.Product.Requests;

public class AddProduct
{
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
}

public class AddProductValidator : AbstractValidator<AddProduct>
{
    public AddProductValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
    }
}