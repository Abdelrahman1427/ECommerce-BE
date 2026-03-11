using Application.Common.OrderDTOS;
using Application.Features.Orders.Queries;
using AutoMapper;
using Domain.Entities;
using Domain.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.Handlers
{
    public class GetOrderDetailsQueryHandler : IRequestHandler<GetOrderDetailsQuery, OrderDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetOrderDetailsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OrderDto> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Repository<Order>()
                .GetQueryableWithIncludes(o => o.OrderItems!, o => o.StatusHistory!)
                .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Order with id {request.Id} not found.");

            if (!request.IsAdmin && order.UserId != request.UserId)
                throw new UnauthorizedAccessException("Access denied.");

            return _mapper.Map<OrderDto>(order);
        }
    }
}
