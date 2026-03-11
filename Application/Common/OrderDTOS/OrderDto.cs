namespace Application.Common.OrderDTOS
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTimeOffset Created { get; set; }
        public AddressDto? ShippingAddress { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Notes { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public List<OrderStatusHistoryDto>? History { get; set; }
    }

    public class OrderStatusHistoryDto
    {
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTimeOffset ChangedAt { get; set; }
        public string ChangedBy { get; set; } = string.Empty;
    }

    public class UpdateOrderStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
