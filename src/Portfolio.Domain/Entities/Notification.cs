using System;
using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities;

public class Notification : AuditableEntity
{
    public string? UserId { get; set; } // Null if it's a global notification (visible to all users / admins)
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "Info"; // Success, Error, Warning, Info
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
    public string? RedirectUrl { get; set; }
    public string? Icon { get; set; }
}
