namespace Application.Common.OrganizationDTOS
{
    public class AddOrganizationDTO
    {
        public required string OrganizationName { get; set; }
        public required string OrganizationPassword { get; set; }
        //public ICollection<Branch>? Branch { get; set; }

    }
}
