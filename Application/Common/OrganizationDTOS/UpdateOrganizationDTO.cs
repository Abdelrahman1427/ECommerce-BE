using Domain.Entities;

namespace Application.Common.OrganizationDTOS
{
    public class UpdateOrganizationDTO
    {
        public required string OrganizationName { get; set; }
        public required string OrganizationPassword { get; set; }
        public ICollection<Branch>? Branch { get; set; }

    }
}
