namespace Application.Common.CartDTOS
{
    public class CartDto
    {
        public string UserId { get; set; } = string.Empty;
        public List<CartItemDto> Items { get; set; } = new();
        public decimal SubTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }

    public class CartItemDto
    {
        public string ItemId { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class CartItemAddDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class CartItemUpdateDto
    {
        public int Quantity { get; set; }
    }
}
