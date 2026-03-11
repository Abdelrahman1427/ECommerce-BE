namespace Application.Common.Auth
{
    public class AuthResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string ExpiresIn { get; set; } = string.Empty;
        public UserInfoDTO User { get; set; } = new();
    }

    public class RefreshTokenDTO
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class RefreshTokenResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public string ExpiresIn { get; set; } = string.Empty;
    }
}
