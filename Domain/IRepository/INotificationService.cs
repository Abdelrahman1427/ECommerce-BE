using Domain.Common;

namespace Domain.IRepository
{
    public interface INotificationService
    {
        Task<NotificationDto> SendAsync(string userId, string title, string message, string type);
        Task<List<NotificationDto>> GetNotificationsAsync(string userId, bool onlyUnread = false);
        Task MarkAsReadAsync(int notificationId);
    }
}
