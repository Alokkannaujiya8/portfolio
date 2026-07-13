using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Portfolio.Application.DTOs.Notification;
using Portfolio.Application.Interfaces.Repositories;
using Portfolio.Application.Interfaces.Services;
using Portfolio.Application.Interfaces.Identity;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly INotificationSender _notificationSender;
    private readonly IEmailService _emailService;
    private readonly IIdentityService _identityService;

    public NotificationService(
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        INotificationSender notificationSender,
        IEmailService emailService,
        IIdentityService identityService)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _notificationSender = notificationSender;
        _emailService = emailService;
        _identityService = identityService;
    }

    public async Task<NotificationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(id);
        return notification == null ? null : _mapper.Map<NotificationDto>(notification);
    }

    public async Task<IEnumerable<NotificationDto>> GetPagedAsync(string? userId, int pageNumber, int pageSize, string? type, bool? isRead, CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationRepository.GetPagedNotificationsAsync(userId, pageNumber, pageSize, type, isRead, cancellationToken);
        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<NotificationDto> CreateAsync(CreateNotificationDto dto, CancellationToken cancellationToken = default)
    {
        var notification = _mapper.Map<Notification>(dto);
        notification.Id = Guid.NewGuid();
        notification.CreatedAt = DateTime.UtcNow;
        notification.IsRead = false;

        await _notificationRepository.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var resultDto = _mapper.Map<NotificationDto>(notification);

        // Real-time SignalR notifications & Emails
        if (!string.IsNullOrEmpty(notification.UserId))
        {
            await _notificationSender.SendNotificationToUserAsync(notification.UserId, resultDto);
            
            var userEmail = await _identityService.GetUserEmailAsync(notification.UserId);
            if (!string.IsNullOrEmpty(userEmail))
            {
                var htmlBody = $@"
                    <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #eee; border-radius: 5px; max-width: 600px;'>
                        <h2 style='color: #4f46e5;'>New Notification</h2>
                        <p>Hello,</p>
                        <p>You have received a new notification on your Portfolio Management System:</p>
                        <div style='background-color: #f9fafb; padding: 15px; border-left: 4px solid #4f46e5; margin: 20px 0;'>
                            <strong style='font-size: 16px;'>{notification.Title}</strong>
                            <p style='margin-top: 10px; color: #374151;'>{notification.Message}</p>
                        </div>
                        {(string.IsNullOrEmpty(notification.RedirectUrl) ? "" : $"<p><a href='{notification.RedirectUrl}' style='background-color: #4f46e5; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>View Details</a></p>")}
                        <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;' />
                        <p style='font-size: 12px; color: #9ca3af;'>This is an automated email. Please do not reply to this email.</p>
                    </div>";
                await _emailService.SendEmailAsync(userEmail, $"New Notification: {notification.Title}", htmlBody);
            }
        }
        else
        {
            // Global Broadcast to everyone + Admin group
            await _notificationSender.SendNotificationToGroupAsync("Admin", resultDto);
            await _notificationSender.SendNotificationToAllAsync(resultDto);
            
            var adminEmails = await _identityService.GetAdminsEmailsAsync();
            foreach (var email in adminEmails)
            {
                var htmlBody = $@"
                    <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #eee; border-radius: 5px; max-width: 600px;'>
                        <h2 style='color: #4f46e5;'>System Notification (Admin)</h2>
                        <p>Hello Admin,</p>
                        <p>A new system-wide notification has been generated:</p>
                        <div style='background-color: #f9fafb; padding: 15px; border-left: 4px solid #ef4444; margin: 20px 0;'>
                            <strong style='font-size: 16px;'>{notification.Title}</strong>
                            <p style='margin-top: 10px; color: #374151;'>{notification.Message}</p>
                        </div>
                        {(string.IsNullOrEmpty(notification.RedirectUrl) ? "" : $"<p><a href='{notification.RedirectUrl}' style='background-color: #4f46e5; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>View Details</a></p>")}
                        <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;' />
                        <p style='font-size: 12px; color: #9ca3af;'>This is an automated email sent to the Admin group.</p>
                    </div>";
                await _emailService.SendEmailAsync(email, $"[Admin] {notification.Title}", htmlBody);
            }
        }

        return resultDto;
    }

    public async Task<NotificationDto?> UpdateAsync(Guid id, UpdateNotificationDto dto, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(id);
        if (notification == null) return null;

        _mapper.Map(dto, notification);
        notification.ModifiedAt = DateTime.UtcNow;

        _notificationRepository.Update(notification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<NotificationDto>(notification);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(id);
        if (notification == null) return false;

        _notificationRepository.Delete(notification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(id);
        if (notification == null) return false;

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;

        _notificationRepository.Update(notification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task MarkAllReadAsync(string userId, CancellationToken cancellationToken = default)
    {
        await _notificationRepository.MarkAllAsReadAsync(userId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> GetUnreadCountAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _notificationRepository.GetUnreadCountAsync(userId, cancellationToken);
    }

    public async Task<IEnumerable<NotificationDto>> GetLatestAsync(string userId, int count, CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationRepository.GetLatestNotificationsAsync(userId, count, cancellationToken);
        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }
}
