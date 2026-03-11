using Application.Common.OrderDTOS;
using Application.Common.Pagination;
using MediatR;

namespace Application.Features.Orders.Queries
{
    public class GetOrdersQuery : PagingDTO, IRequest<PagedResponse<OrderDto>>
    {
        public string? UserId { get; set; }
        public bool IsAdmin { get; set; }
        public string? Status { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
    }
}
