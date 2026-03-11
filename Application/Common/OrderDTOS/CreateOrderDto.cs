namespace Application.Common.OrderDTOS
{
    public class CreateOrderDto
    {
        public AddressDto ShippingAddress { get; set; } = new();
        public string PaymentMethod { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? DiscountCode { get; set; }
        public List<CreateOrderItemDto>? Items { get; set; }
        public bool UseCart { get; set; } = false;
    }

    public class AddressDto
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? State { get; set; }
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}
