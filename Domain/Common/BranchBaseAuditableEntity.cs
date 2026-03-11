using Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Common
{
    public abstract class BranchBaseAuditableEntity : BaseAuditableEntity, IBranchEntity
    {
        [ForeignKey(nameof(Branch))]
        public int? BranchId { get; set; }
        public Branch? Branch { get; set; }

    }
}
