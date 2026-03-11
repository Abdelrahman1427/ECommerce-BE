
using Domain.Common;
using Domain.Entities;
using Domain.Enum;
using Domain.IRepository;
using Infrastructure.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IUnitOfWork unitOfWork, IHubContext<NotificationHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        public async Task<PagedResponse<NotificationDto>> GetAllAsync(string userId, PagingDTO paging, bool? onlyUnread)
        {
            var query = _unitOfWork.Repository<Notification>()
                .GetQueryable()
                .Where(n => n.UserId == userId);

            if (onlyUnread == true)
                query = query.Where(n => !n.IsRead);
            else if (onlyUnread == false)
                query = query.Where(n => n.IsRead);

            query = query.OrderByDescending(n => n.Created);

            var paged = await query.ToPagedResponseAsync(paging);
            var dtos = paged.Items.Select(MapToDto).ToList();
            return new PagedResponse<NotificationDto>(dtos, paged.TotalCount, paged.PageNumber, paged.PageSize);
        }

        public async Task<NotificationDto?> GetByIdAsync(int id)
        {
            var n = await _unitOfWork.Repository<Notification>().GetByIdAsync(id, CancellationToken.None);
            return n == null ? null : MapToDto(n);
        }

        public async Task<NotificationDto> SendAsync(string userId, string title, string message, string type)
        {
            if (!Enum.TryParse<NotificationType>(type, true, out var notificationType))
                notificationType = NotificationType.Info;

            var entity = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = notificationType,
                IsRead = false
            };

            await _unitOfWork.Repository<Notification>().AddAsync(entity, CancellationToken.None);
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);

            var dto = MapToDto(entity);
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", dto);
            return dto;
        }

        public async Task MarkAsReadAsync(int id)
        {
            var n = await _unitOfWork.Repository<Notification>().GetByIdAsync(id, CancellationToken.None);
            if (n == null) return;
            n.IsRead = true;
            await _unitOfWork.Repository<Notification>().UpdateAsync(n, CancellationToken.None);
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);
        }

        public async Task MarkAsUnreadAsync(int id)
        {
            var n = await _unitOfWork.Repository<Notification>().GetByIdAsync(id, CancellationToken.None);
            if (n == null) return;
            n.IsRead = false;
            await _unitOfWork.Repository<Notification>().UpdateAsync(n, CancellationToken.None);
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            var notifications = await _unitOfWork.Repository<Notification>()
                .GetQueryable(false)
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var n in notifications)
                n.IsRead = true;

            await _unitOfWork.Repository<Notification>().UpdateRange(notifications, CancellationToken.None);
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);
        }

        public async Task<int> GetUnreadCountAsync(string userId)
            => await _unitOfWork.Repository<Notification>()
                .GetQueryable()
                .CountAsync(n => n.UserId == userId && !n.IsRead);

        private static NotificationDto MapToDto(Notification n) => new()
        {
            Id = n.Id,
            UserId = n.UserId,
            Title = n.Title,
            Message = n.Message,
            Type = n.Type.ToString(),
            IsRead = n.IsRead,
            Created = n.Created
        };
    }
}
