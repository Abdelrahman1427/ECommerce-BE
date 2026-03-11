using Application.Features.Orders.Commands;
using FluentValidation;

namespace Application.Features.Orders.Validators
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.Order.UserId).NotEmpty().WithMessage("UserId is required.");
            RuleFor(x => x.Order.Items).NotEmpty().WithMessage("Order must contain at least one item.");
            RuleForEach(x => x.Order.Items).SetValidator(new CreateOrderItemValidator());
        }

        private class CreateOrderItemValidator : AbstractValidator<Application.Common.OrderDTOS.CreateOrderItemDto>
        {
            public CreateOrderItemValidator()
            {
                RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("ProductId is required.");
                RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0.");
            }
        }
    }
}
