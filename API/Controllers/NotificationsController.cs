using Application.Common.NotificationDTOS;
using Application.Common.Pagination;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        //[HttpGet]
        //public async Task<IActionResult> Get([FromQuery] PagingDTO paging, [FromQuery] bool? onlyUnread = null)
        //{
        //    var result = await _notificationService.GetAllAsync(UserId, paging, onlyUnread);
        //    return Ok(new { success = true, data = result });
        //}

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetById(int id)
        //{
        //    var result = await _notificationService.GetByIdAsync(id);
        //    return result == null ? NotFound() : Ok(new { success = true, data = result });
        //}

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return Ok(new { success = true, message = "Notification marked as read" });
        }

        //[HttpPost("{id}/unread")]
        //public async Task<IActionResult> MarkAsUnread(int id)
        //{
        //    await _notificationService.MarkAsUnreadAsync(id);
        //    return Ok(new { success = true, message = "Notification marked as unread" });
        //}

        //[HttpPost("mark-all-read")]
        //public async Task<IActionResult> MarkAllAsRead()
        //{
        //    await _notificationService.MarkAllAsReadAsync(UserId);
        //    return Ok(new { success = true, message = "All notifications marked as read" });
        //}

        //[HttpGet("unread-count")]
        //public async Task<IActionResult> UnreadCount()
        //{
        //    var count = await _notificationService.GetUnreadCountAsync(UserId);
        //    return Ok(new { success = true, data = new { count } });
        //}

        [HttpPost("send")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Send([FromBody] SendNotificationDto dto)
        {
            var result = await _notificationService.SendAsync(dto.UserId, dto.Title, dto.Message, dto.Type);
            return Ok(new { success = true, data = result });
        }
    }
}
