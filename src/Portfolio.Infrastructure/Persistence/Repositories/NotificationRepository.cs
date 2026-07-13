using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Entities;
using Portfolio.Application.Interfaces.Repositories;
using Portfolio.Infrastructure.Persistence.Context;

namespace Portfolio.Infrastructure.Persistence.Repositories;

public class NotificationRepository : Repository<Notification>, INotificationRepository
{
    public NotificationRepository(PortfolioDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Notification>> GetPagedNotificationsAsync(string? userId, int pageNumber, int pageSize, string? type, bool? isRead, CancellationToken cancellationToken = default)
    {
        var query = _context.Notifications.AsQueryable();

        if (!string.IsNullOrEmpty(userId))
        {
            // Fetch notifications targeted specifically to the user OR global ones (UserId is null)
            query = query.Where(n => n.UserId == userId || n.UserId == null);
        }

        if (!string.IsNullOrEmpty(type))
        {
            query = query.Where(n => n.Type == type);
        }

        if (isRead.HasValue)
        {
            query = query.Where(n => n.IsRead == isRead.Value);
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUnreadCountAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .CountAsync(n => (n.UserId == userId || n.UserId == null) && !n.IsRead, cancellationToken);
    }

    public async Task<IEnumerable<Notification>> GetLatestNotificationsAsync(string userId, int count, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId || n.UserId == null)
            .OrderByDescending(n => n.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task MarkAllAsReadAsync(string userId, CancellationToken cancellationToken = default)
    {
        var unreadNotifications = await _context.Notifications
            .Where(n => (n.UserId == userId || n.UserId == null) && !n.IsRead)
            .ToListAsync(cancellationToken);

        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            _context.Notifications.Entry(notification).State = EntityState.Modified;
        }
    }
}
