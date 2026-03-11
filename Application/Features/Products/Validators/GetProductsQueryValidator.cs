using Application.Features.Products.Queries;
using FluentValidation;

namespace Application.Features.Products.Validators
{
    public class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
    {
        public GetProductsQueryValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThan(0).WithMessage("Page number must be greater than 0.");
            RuleFor(x => x.PageSize).GreaterThan(0).WithMessage("Page size must be greater than 0.");
            RuleFor(x => x.SortBy).Must(s => string.IsNullOrWhiteSpace(s) || new[] { "name", "price" }.Contains(s.ToLower()))
                .WithMessage("SortBy must be either 'name' or 'price'.");
        }
    }
}
