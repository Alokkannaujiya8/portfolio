using System.Threading.Tasks;
using Portfolio.Application.DTOs.Notification;

namespace Portfolio.Application.Interfaces.Services;

public interface INotificationSender
{
    Task SendNotificationToUserAsync(string userId, NotificationDto notification);
    Task SendNotificationToAllAsync(NotificationDto notification);
    Task SendNotificationToGroupAsync(string groupName, NotificationDto notification);
}
