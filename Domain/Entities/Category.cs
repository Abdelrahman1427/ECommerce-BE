using Domain.Common;

namespace Domain.Entities
{
    public class Category : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; }

        public bool IsActive { get; set; } = true;
        public ICollection<Product>? Products { get; set; }
    }
}
