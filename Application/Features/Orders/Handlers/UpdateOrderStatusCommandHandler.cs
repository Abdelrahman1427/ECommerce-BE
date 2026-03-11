using Application.Common.OrderDTOS;
using Application.Features.Orders.Commands;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enum;
using Domain.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.Handlers
{
    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, OrderDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public UpdateOrderStatusCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<OrderDto> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            if (!Enum.TryParse<OrderStatus>(request.Status, true, out var newStatus))
                throw new ArgumentException($"Invalid status: {request.Status}");

            var order = await _unitOfWork.Repository<Order>()
                .GetQueryableWithIncludes(o => o.OrderItems!, o => o.StatusHistory!)
                .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Order {request.Id} not found.");

            order.Status = newStatus;
            order.StatusHistory ??= new List<OrderStatusHistory>();
            order.StatusHistory.Add(new OrderStatusHistory
            {
                Status = newStatus,
                Notes = request.Notes,
                ChangedBy = request.ChangedBy,
                ChangedAt = DateTimeOffset.UtcNow
            });

            await _unitOfWork.Repository<Order>().UpdateAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _notificationService.SendAsync(order.UserId, "Order Status Updated",
                $"Your order #{order.OrderNumber} status changed to {newStatus}.", "Info");

            return _mapper.Map<OrderDto>(order);
        }
    }
}
