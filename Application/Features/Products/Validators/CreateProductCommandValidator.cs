using Application.Features.Products.Commands;
using FluentValidation;

namespace Application.Features.Products.Validators
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Product.Name).NotEmpty().WithMessage("Product name is required.");
            RuleFor(x => x.Product.Price).GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0.");
            RuleFor(x => x.Product.StockQuantity).GreaterThanOrEqualTo(0).WithMessage("Stock quantity must be greater than or equal to 0.");
            RuleFor(x => x.Product.CategoryId).GreaterThan(0).WithMessage("Category is required.");
        }
    }
}
