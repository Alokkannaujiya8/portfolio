using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Portfolio.Application.DTOs.Notification;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.Infrastructure.SignalR;

public class NotificationSender : INotificationSender
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationSender(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotificationToUserAsync(string userId, NotificationDto notification)
    {
        await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", notification);
    }

    public async Task SendNotificationToAllAsync(NotificationDto notification)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
    }

    public async Task SendNotificationToGroupAsync(string groupName, NotificationDto notification)
    {
        await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", notification);
    }
}
