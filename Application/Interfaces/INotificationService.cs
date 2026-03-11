using Application.Common.NotificationDTOS;
using Application.Common.Pagination;

namespace Application.Interfaces
{
    public interface INotificationService
    {
        Task<PagedResponse<NotificationDto>> GetAllAsync(string userId, PagingDTO paging, bool? onlyUnread);
        Task<NotificationDto?> GetByIdAsync(int id);
        Task<NotificationDto> SendAsync(string userId, string title, string message, string type);
        Task MarkAsReadAsync(int id);
        Task MarkAsUnreadAsync(int id);
        Task MarkAllAsReadAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
    }
}
