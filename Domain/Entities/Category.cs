using Domain.Common;

namespace Domain.Entities
{
    public class Category : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;

        public ICollection<Product>? Products { get; set; }
    }
}
