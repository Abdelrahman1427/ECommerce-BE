using Application.Common.OrderDTOS;
using Application.Features.Orders.Commands;
using Application.Interfaces;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enum;
using Domain.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.Handlers
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public CreateOrderCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var userId = request.UserId;
            var dto = request.Order;

            // 1. Resolve items (cart or direct)
            var itemsToOrder = dto.UseCart
                ? CartService.GetCartItems(userId).Select(c => new CreateOrderItemDto { ProductId = c.ProductId, Quantity = c.Quantity }).ToList()
                : dto.Items ?? new List<CreateOrderItemDto>();

            if (!itemsToOrder.Any())
                throw new ArgumentException("Order must contain at least one item.");

            // 2. Validate stock
            var productRepo = _unitOfWork.Repository<Product>();
            var productIds = itemsToOrder.Select(i => i.ProductId).Distinct().ToList();
            var products = await productRepo.GetQueryable(false).Where(p => productIds.Contains(p.Id)).ToListAsync(cancellationToken);

            if (products.Count != productIds.Count)
                throw new KeyNotFoundException("One or more products not found.");

            foreach (var item in itemsToOrder)
            {
                var product = products.Single(p => p.Id == item.ProductId);
                if (!product.IsActive)
                    throw new InvalidOperationException($"Product '{product.Name}' is not available.");
                if (product.StockQuantity < item.Quantity)
                    throw new InvalidOperationException($"Insufficient stock for '{product.Name}'. Available: {product.StockQuantity}");
            }

            // 3. Apply discount if code provided
            decimal discountAmount = 0;
            if (!string.IsNullOrWhiteSpace(dto.DiscountCode))
            {
                var discount = await _unitOfWork.Repository<Discount>()
                    .GetQueryable(false)
                    .FirstOrDefaultAsync(d => d.Code == dto.DiscountCode.ToUpper(), cancellationToken);

                if (discount != null && discount.IsActive
                    && (discount.EndDate == null || discount.EndDate > DateTimeOffset.UtcNow)
                    && (discount.MaxUsage == null || discount.UsedCount < discount.MaxUsage))
                {
                    // calculate after we know total
                    // Will apply below after computing total
                }
            }

            // 4. Build order items + compute total
            var orderItems = new List<OrderItem>();
            decimal total = 0;

            foreach (var item in itemsToOrder)
            {
                var product = products.Single(p => p.Id == item.ProductId);
                var lineTotal = product.Price * item.Quantity;
                total += lineTotal;
                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = item.Quantity,
                    Price = product.Price,
                    Total = lineTotal
                });
                product.StockQuantity -= item.Quantity;
                await productRepo.UpdateAsync(product, cancellationToken);
            }

            // 5. Apply discount amount
            if (!string.IsNullOrWhiteSpace(dto.DiscountCode))
            {
                var discount = await _unitOfWork.Repository<Discount>()
                    .GetQueryable(false)
                    .FirstOrDefaultAsync(d => d.Code == dto.DiscountCode.ToUpper(), cancellationToken);

                if (discount != null && discount.IsActive)
                {
                    discountAmount = discount.Type switch
                    {
                        Domain.Enum.DiscountType.Percentage => Math.Round(total * discount.Value / 100, 2),
                        Domain.Enum.DiscountType.FixedAmount => Math.Min(discount.Value, total),
                        _ => 0
                    };
                    discount.UsedCount++;
                    await _unitOfWork.Repository<Discount>().UpdateAsync(discount, cancellationToken);
                }
            }

            // 6. Create order
            var order = new Order
            {
                UserId = userId,
                OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}",
                Status = OrderStatus.Pending,
                ShippingStreet = dto.ShippingAddress.Street,
                ShippingCity = dto.ShippingAddress.City,
                ShippingState = dto.ShippingAddress.State,
                ShippingPostalCode = dto.ShippingAddress.PostalCode,
                ShippingCountry = dto.ShippingAddress.Country,
                PaymentMethod = dto.PaymentMethod,
                Notes = dto.Notes,
                DiscountCode = dto.DiscountCode,
                TotalAmount = total,
                Discount = discountAmount,
                FinalAmount = total - discountAmount,
                OrderItems = orderItems,
                StatusHistory = new List<OrderStatusHistory>
                {
                    new() { Status = OrderStatus.Pending, ChangedBy = userId, Notes = "Order placed", ChangedAt = DateTimeOffset.UtcNow }
                }
            };

            await _unitOfWork.Repository<Order>().AddAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 7. Clear cart if used
            if (dto.UseCart)
                CartService.ClearUserCart(userId);

            // 8. Notify user
            await _notificationService.SendAsync(userId, "Order Confirmed",
                $"Your order #{order.OrderNumber} has been placed successfully.", "Success");

            return _mapper.Map<OrderDto>(order);
        }
    }
}
