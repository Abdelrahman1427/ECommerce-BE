using Application.Common.OrderDTOS;
using Application.Common.Pagination;
using Application.Features.Orders.Queries;
using Application.Helper;
using AutoMapper;
using Domain.Entities;
using Domain.Enum;
using Domain.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.Handlers
{
    public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, PagedResponse<OrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetOrdersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Repository<Order>()
                .GetQueryableWithIncludes(o => o.OrderItems!, o => o.StatusHistory!)
                .AsNoTracking();

            if (!request.IsAdmin && !string.IsNullOrWhiteSpace(request.UserId))
                query = query.Where(o => o.UserId == request.UserId);

            if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<OrderStatus>(request.Status, true, out var status))
                query = query.Where(o => o.Status == status);

            if (!string.IsNullOrWhiteSpace(request.FromDate) && DateTimeOffset.TryParse(request.FromDate, out var from))
                query = query.Where(o => o.Created >= from);

            if (!string.IsNullOrWhiteSpace(request.ToDate) && DateTimeOffset.TryParse(request.ToDate, out var to))
                query = query.Where(o => o.Created <= to.AddDays(1));

            query = query.OrderByDescending(o => o.Created);

            var paged = await query.ToPagedResponseAsync(request);
            var dtos = paged.Items.Select(o => _mapper.Map<OrderDto>(o)).ToList();
            return new PagedResponse<OrderDto>(dtos, paged.TotalCount, paged.PageNumber, paged.PageSize);
        }
    }
}
