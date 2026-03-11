namespace Application.Common.Auth
{
    public class UserInfoDTO
    {
        public string Id { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;

        public UserInfoDTO() { }
        public UserInfoDTO(string id, string username, string email, string role)
        {
            Id = id; Username = username; Email = email; Role = role;
        }
    }
}
