using Application.Common.BranchDTOS;

namespace Application.Common.OrganizationDTOS
{
    public class GetOrganizationDTO
    {
        public int? Id { get; set; }
        public string? OrganizationName { get; set; }
        public string? OrganizationPassword { get; set; }
        //public ICollection<GetBranchDTO> Branch { get; set; }
        public IEnumerable<GetBranchDTO> Branches { get; set; } = new List<GetBranchDTO>();

    }
}
