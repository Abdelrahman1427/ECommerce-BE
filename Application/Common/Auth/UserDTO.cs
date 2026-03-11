namespace Application.Common.Auth
{
    public class UserDTO
    {
        public string Id { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? phoneNumber { get; set; }
        public string? BranchName { get; set; }
        public string? RoleName { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
