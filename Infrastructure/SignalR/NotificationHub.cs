using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.SignalR
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string userId, object message)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
    }
}
