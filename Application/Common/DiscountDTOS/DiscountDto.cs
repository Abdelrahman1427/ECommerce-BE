namespace Application.Common.DiscountDTOS
{
    public class DiscountDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public int? MaxUsage { get; set; }
        public int UsedCount { get; set; }
        public decimal? MinimumOrderTotal { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset Created { get; set; }
    }

    public class AddDiscountDTO
    {
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public int? MaxUsage { get; set; }
        public decimal? MinimumOrderTotal { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateDiscountDTO
    {
        public string? Description { get; set; }
        public string? Type { get; set; }
        public decimal? Value { get; set; }
        public int? MaxUsage { get; set; }
        public decimal? MinimumOrderTotal { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsActive { get; set; }
    }

    public class ApplyDiscountDto
    {
        public int OrderId { get; set; }
        public string Code { get; set; } = string.Empty;
    }

    public class ApplyDiscountResultDto
    {
        public bool Applied { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NewTotal { get; set; }
        public string? Message { get; set; }
    }
}
