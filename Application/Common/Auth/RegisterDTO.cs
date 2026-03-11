namespace Application.Common.Auth
{
    public class RegisterDTO
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public string Password { get; set; } = string.Empty;
        public string? Role { get; set; } = "User";
    }
}
