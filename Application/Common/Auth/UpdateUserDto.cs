namespace Application.Common.Auth
{
    public class UpdateUserDto

    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int? BranchId { get; set; }
        public string? RoleId { get; set; }
        public bool? IsConfirmed { get; set; }

    }
}
