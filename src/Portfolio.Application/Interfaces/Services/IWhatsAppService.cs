using System.Threading;
using System.Threading.Tasks;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Interfaces.Services;

/// <summary>
/// Service to send notifications via WhatsApp.
/// </summary>
public interface IWhatsAppService
{
    /// <summary>
    /// Sends a WhatsApp notification when a new contact message is received.
    /// </summary>
    /// <param name="contactMessage">The contact message details.</param>
    /// <param name="cancellationToken">Token to cancel execution.</param>
    /// <returns>True if the notification was sent successfully, otherwise false.</returns>
    Task<bool> SendContactInquiryNotificationAsync(ContactMessage contactMessage, CancellationToken cancellationToken = default);
}
