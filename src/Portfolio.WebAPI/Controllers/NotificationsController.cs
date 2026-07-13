using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.DTOs.Notification;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.WebAPI.Controllers;

[Authorize]
[Route("api/v1/notifications")]
public class NotificationsController : ApiControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? type = null,
        [FromQuery] bool? isRead = null,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var isUserAdmin = User.IsInRole("Admin");
        
        // Admins see all notifications when targetUserId is null, standard users see their own + global notifications
        var targetUserId = isUserAdmin ? null : userId;
        var notifications = await _notificationService.GetPagedAsync(targetUserId, pageNumber, pageSize, type, isRead, cancellationToken);
        return Ok(notifications);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var count = await _notificationService.GetUnreadCountAsync(userId, cancellationToken);
        return Ok(new { Count = count });
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatest([FromQuery] int count = 5, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var notifications = await _notificationService.GetLatestAsync(userId, count, cancellationToken);
        return Ok(notifications);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var notification = await _notificationService.GetByIdAsync(id, cancellationToken);
        if (notification == null) return NotFound();

        var userId = GetCurrentUserId();
        if (!User.IsInRole("Admin") && notification.UserId != null && notification.UserId != userId)
        {
            return Forbid();
        }

        return Ok(notification);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateNotificationDto dto, CancellationToken cancellationToken)
    {
        var validator = new CreateNotificationDtoValidator();
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var notification = await _notificationService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = notification.Id }, notification);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNotificationDto dto, CancellationToken cancellationToken)
    {
        var validator = new UpdateNotificationDtoValidator();
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var updated = await _notificationService.UpdateAsync(id, dto, cancellationToken);
        if (updated == null) return NotFound();

        return Ok(updated);
    }

    [HttpPut("{id}/mark-read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken cancellationToken)
    {
        var notification = await _notificationService.GetByIdAsync(id, cancellationToken);
        if (notification == null) return NotFound();

        var userId = GetCurrentUserId();
        if (!User.IsInRole("Admin") && notification.UserId != null && notification.UserId != userId)
        {
            return Forbid();
        }

        var result = await _notificationService.MarkAsReadAsync(id, cancellationToken);
        return result ? Ok(new { Success = true }) : BadRequest();
    }

    [HttpPut("mark-all-read")]
    public async Task<IActionResult> MarkAllRead(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        await _notificationService.MarkAllReadAsync(userId, cancellationToken);
        return Ok(new { Success = true });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var notification = await _notificationService.GetByIdAsync(id, cancellationToken);
        if (notification == null) return NotFound();

        var userId = GetCurrentUserId();
        if (!User.IsInRole("Admin") && notification.UserId != null && notification.UserId != userId)
        {
            return Forbid();
        }

        var result = await _notificationService.DeleteAsync(id, cancellationToken);
        return result ? Ok(new { Success = true }) : BadRequest();
    }

    private string GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User is not authenticated.");
    }
}
