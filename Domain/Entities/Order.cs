using Domain.Common;
using Domain.Enum;

namespace Domain.Entities
{
    public class Order : BaseAuditableEntity
    {
        public string UserId { get; set; } = string.Empty;

        public string OrderNumber { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalAmount { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // Shipping Address
        public string ShippingStreet { get; set; } = string.Empty;
        public string ShippingCity { get; set; } = string.Empty;
        public string? ShippingState { get; set; }
        public string ShippingPostalCode { get; set; } = string.Empty;
        public string ShippingCountry { get; set; } = string.Empty;

        public string? PaymentMethod { get; set; }
        public string? Notes { get; set; }
        public string? DiscountCode { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; }
        public ICollection<OrderStatusHistory>? StatusHistory { get; set; }
    }

    public class OrderStatusHistory : BaseEntity
    {
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public OrderStatus Status { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset ChangedAt { get; set; } = DateTimeOffset.UtcNow;
        public string ChangedBy { get; set; } = string.Empty;
    }
}
