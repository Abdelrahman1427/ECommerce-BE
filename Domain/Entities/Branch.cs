using System.Diagnostics.Metrics;
using Domain.Common;

namespace Domain.Entities
{
    public class Branch : BaseAuditableEntity
    {

        //new
        public string Name { get; set; }
        public string Code { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Address { get; set; }
        public string WorkingHours { get; set; }
        public bool IsActive { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }

    }
}
