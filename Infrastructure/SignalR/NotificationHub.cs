using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Infrastructure.SignalR
{

    public class UserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }

    [Authorize]
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string userId, object message)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}