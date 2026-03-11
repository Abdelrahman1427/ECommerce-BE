using Application.Common.OrderDTOS;
using MediatR;

namespace Application.Features.Orders.Commands
{
    public class CreateOrderCommand : IRequest<OrderDto>
    {
        public string UserId { get; set; } = string.Empty;  // set from JWT in controller
        public CreateOrderDto Order { get; set; } = new();
    }

    public class UpdateOrderStatusCommand : IRequest<OrderDto>
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string ChangedBy { get; set; } = string.Empty;
    }
}
