using Domain.Common;
using Domain.Enum;

namespace Domain.Entities
{
    public class Notification : BaseAuditableEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; } = NotificationType.Info;
        public bool IsRead { get; set; }

        // Convenience property for required contract
        public DateTimeOffset CreatedAt => Created;
    }
}
