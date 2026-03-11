using Domain.Common;

namespace Domain.Entities
{
    public class Product : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; } = true;
        public string? ImageUrls { get; set; }  // comma-separated

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
