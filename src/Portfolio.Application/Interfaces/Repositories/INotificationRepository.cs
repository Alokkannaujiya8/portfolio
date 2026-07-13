using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Interfaces.Repositories;

public interface INotificationRepository : IRepository<Notification>
{
    Task<IEnumerable<Notification>> GetPagedNotificationsAsync(string? userId, int pageNumber, int pageSize, string? type, bool? isRead, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetLatestNotificationsAsync(string userId, int count, CancellationToken cancellationToken = default);
    Task MarkAllAsReadAsync(string userId, CancellationToken cancellationToken = default);
}
