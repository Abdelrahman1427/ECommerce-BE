using Application.Common.Auth;
using System.Security.Claims;

namespace Application.Interfaces.IAuth
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> LoginAsync(LoginDTO model);
        Task<(bool IsSuccess, string Message)> RegisterAsync(RegisterDTO model);
        Task<UserInfoDTO?> GetUserInfoAsync(ClaimsPrincipal userPrincipal);
        Task<RefreshTokenResponseDTO> RefreshTokenAsync(string refreshToken);
        Task RevokeTokenAsync(string refreshToken);
        Task<IEnumerable<RoleDTO>> GetRolesAsync();
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<(bool IsSuccess, string Message)> UpdateUserAsync(string id, UpdateUserDto model);
        Task<(bool IsSuccess, string Message)> DeleteUserAsync(string id);
        Task<List<UserLookUpDTO>> GetLookup();
    }
}
