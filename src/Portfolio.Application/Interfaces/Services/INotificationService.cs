using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Portfolio.Application.DTOs.Notification;

namespace Portfolio.Application.Interfaces.Services;

public interface INotificationService
{
    Task<NotificationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationDto>> GetPagedAsync(string? userId, int pageNumber, int pageSize, string? type, bool? isRead, CancellationToken cancellationToken = default);
    Task<NotificationDto> CreateAsync(CreateNotificationDto dto, CancellationToken cancellationToken = default);
    Task<NotificationDto?> UpdateAsync(Guid id, UpdateNotificationDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default);
    Task MarkAllReadAsync(string userId, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationDto>> GetLatestAsync(string userId, int count, CancellationToken cancellationToken = default);
}
