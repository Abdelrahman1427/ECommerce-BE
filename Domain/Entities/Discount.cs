using Domain.Common;
using Domain.Enum;

namespace Domain.Entities
{
    public class Discount : BaseAuditableEntity
    {
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DiscountType Type { get; set; }
        public decimal Value { get; set; }
        public int? MaxUsage { get; set; }
        public int UsedCount { get; set; }
        public decimal? MinimumOrderTotal { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
