using Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class ApplicationUser : IdentityUser, IBranchEntity
    {
        public bool IsConfirmed { get; set; } = false;
        public string FullName { get; set; } = string.Empty;

        public int? BranchId { get; set; }
        public Branch? Branch { get; set; }
    }
}
