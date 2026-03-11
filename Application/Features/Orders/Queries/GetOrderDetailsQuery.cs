using Application.Common.OrderDTOS;
using MediatR;

namespace Application.Features.Orders.Queries
{
    public class GetOrderDetailsQuery : IRequest<OrderDto>
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
    }
}
