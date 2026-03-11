namespace Application.Common.BranchDTOS
{
    public class BranchLookUp
    {
        public required int Id { get; set; }
        public required string BranchName { get; set; }
        public required string PhoneNumber { get; set; }
        public required string BranchAddress { get; set; }
    }
}
