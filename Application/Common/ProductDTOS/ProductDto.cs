namespace Application.Common.ProductDTOS
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public List<string>? Images { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}
