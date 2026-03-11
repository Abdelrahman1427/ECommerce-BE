using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Common.Auth;
using Application.Interfaces.IAuth;
using AutoMapper;
using Domain.Entities;
using Domain.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        // In-memory refresh token store (production: use DB or Redis)
        private static readonly Dictionary<string, (string UserId, DateTime Expiry)> _refreshTokens = new();

        public AuthService(UserManager<ApplicationUser> userManager,
                           RoleManager<IdentityRole> roleManager,
                           IUnitOfWork unitOfWork, IMapper mapper,
                           IConfiguration config)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<UserLookUpDTO>> GetLookup()
        {
            var users = _unitOfWork.Repository<ApplicationUser>()
                .GetQueryable(false)
                .Select(s => new UserLookUpDTO { Id = s.Id, FullName = s.FullName })
                .ToList();
            return _mapper.Map<List<UserLookUpDTO>>(users);
        }

        public async Task<(bool IsSuccess, string Message)> RegisterAsync(RegisterDTO model)
        {
            var emailExists = await _userManager.FindByEmailAsync(model.Email);
            if (emailExists != null)
                return (false, "Email already registered.");

            var user = new ApplicationUser
            {
                UserName = model.Email,
                FullName = model.FullName,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

            var roleName = model.Role ?? "User";
            if (!await _roleManager.RoleExistsAsync(roleName))
                roleName = "User";

            await _userManager.AddToRoleAsync(user, roleName);
            return (true, "Registered successfully.");
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDTO model)
        {
            // Frontend sends email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                throw new UnauthorizedAccessException("Invalid email or password.");

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName!),
                new(ClaimTypes.Email, user.Email!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var token = GenerateToken(claims);
            var refreshToken = GenerateRefreshToken(user.Id);

            return new AuthResponseDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                ExpiresIn = token.ValidTo.ToString("o"),
                User = new UserInfoDTO(user.Id, user.UserName!, user.Email!, roles.FirstOrDefault() ?? "User")
            };
        }

        public async Task<UserInfoDTO?> GetUserInfoAsync(ClaimsPrincipal userPrincipal)
        {
            var userId = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return null;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            return new UserInfoDTO(user.Id, user.UserName!, user.Email!, roles.FirstOrDefault() ?? "User");
        }

        public async Task<RefreshTokenResponseDTO> RefreshTokenAsync(string refreshToken)
        {
            if (!_refreshTokens.TryGetValue(refreshToken, out var entry) || entry.Expiry < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            _refreshTokens.Remove(refreshToken);

            var user = await _userManager.FindByIdAsync(entry.UserId)
                ?? throw new UnauthorizedAccessException("User not found.");

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName!),
                new(ClaimTypes.Email, user.Email!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var token = GenerateToken(claims);
            var newRefreshToken = GenerateRefreshToken(user.Id);

            return new RefreshTokenResponseDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresIn = token.ValidTo.ToString("o")
            };
        }

        public Task RevokeTokenAsync(string refreshToken)
        {
            _refreshTokens.Remove(refreshToken);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<RoleDTO>> GetRolesAsync()
            => await Task.FromResult(_roleManager.Roles.Select(r => new RoleDTO { Id = r.Id, Name = r.Name }).ToList());

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<UserDTO>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new UserDTO
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    RoleName = roles.FirstOrDefault(),
                    IsConfirmed = user.EmailConfirmed
                });
            }
            return result;
        }

        public async Task<(bool IsSuccess, string Message)> UpdateUserAsync(string id, UpdateUserDto model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return (false, "User not found.");

            user.UserName = model.Username;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return (false, "Failed to update user.");

            if (!string.IsNullOrWhiteSpace(model.RoleId))
            {
                var role = await _roleManager.FindByIdAsync(model.RoleId);
                if (role == null) return (false, "Invalid role ID.");
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, role.Name!);
            }

            return (true, "User updated successfully.");
        }

        public async Task<(bool IsSuccess, string Message)> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return (false, "User not found.");

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any()) await _userManager.RemoveFromRolesAsync(user, roles);

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded ? (true, "User deleted successfully.") : (false, "Failed to delete user.");
        }

        private JwtSecurityToken GenerateToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]!));
            return new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                expires: DateTime.UtcNow.AddHours(8),
                claims: claims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
        }

        private string GenerateRefreshToken(string userId)
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            _refreshTokens[token] = (userId, DateTime.UtcNow.AddDays(7));
            return token;
        }
    }
}
