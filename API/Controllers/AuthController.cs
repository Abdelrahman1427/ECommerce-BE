using Application.Common.Auth;
using Application.Interfaces.IAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            var result = await _authService.RegisterAsync(model);
            return result.IsSuccess
                ? Ok(new { success = true, message = result.Message })
                : BadRequest(new { success = false, message = result.Message });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            var result = await _authService.LoginAsync(model);
            return Ok(new { success = true, data = result });
        }

        // GET /api/auth/me  — frontend calls this after login
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            var result = await _authService.GetUserInfoAsync(User);
            return result == null ? Unauthorized() : Ok(new { success = true, data = result });
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDTO dto)
        {
            var result = await _authService.RefreshTokenAsync(dto.RefreshToken);
            return Ok(new { success = true, data = result });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenDTO dto)
        {
            await _authService.RevokeTokenAsync(dto.RefreshToken);
            return Ok(new { success = true });
        }

        [HttpGet("roles")]
        [Authorize]
        public async Task<IActionResult> GetRoles() => Ok(await _authService.GetRolesAsync());

        [HttpGet("users")]
        [Authorize]
        public async Task<IActionResult> GetUsers() => Ok(await _authService.GetAllUsersAsync());

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(string id, UpdateUserDto model)
        {
            var result = await _authService.UpdateUserAsync(id, model);
            return result.IsSuccess
                ? Ok(new { success = true, message = result.Message })
                : BadRequest(new { success = false, message = result.Message });
        }

        [HttpDelete("delete-user/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _authService.DeleteUserAsync(id);
            return result.IsSuccess
                ? Ok(new { success = true, message = result.Message })
                : BadRequest(new { success = false, message = result.Message });
        }

        [HttpGet("UserLookup")]
        [Authorize]
        public async Task<ActionResult<List<UserLookUpDTO>>> UserLookup()
            => Ok(await _authService.GetLookup());
    }
}
