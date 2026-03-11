namespace Application.Common.BranchDTOS
{
    public class AddBranchDTO
    {
        //public required string BranchName { get; set; }
        //public required string Email { get; set; }
        //public required string PhoneNumber { get; set; }
        //public required string UserName { get; set; }
        //public required string BranchPassword { get; set; }
        //public int? OrganizationId { get; set; }

        public required string BranchName { get; set; }
        //public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        //public required string UserName { get; set; }
        public required int FloorCount { get; set; }

        public required string BranchAddress { get; set; }

    }
}
