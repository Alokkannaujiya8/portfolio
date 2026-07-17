namespace Portfolio.Application.Common.Configurations;

/// <summary>
/// Settings for WhatsApp notification provider.
/// </summary>
public class WhatsAppSettings
{
    public const string SectionName = "WhatsApp";

    /// <summary>
    /// The API provider (e.g., "Meta" or "Twilio").
    /// </summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// Access token or Auth Token for authentication.
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Phone number ID (Meta) or Account SID (Twilio).
    /// </summary>
    public string PhoneNumberId { get; set; } = string.Empty;

    /// <summary>
    /// The recipient's phone number with country code.
    /// </summary>
    public string RecipientNumber { get; set; } = string.Empty;

    /// <summary>
    /// API Version (Meta Cloud API, e.g., "v20.0") or Sender Number (Twilio, e.g., "whatsapp:+14155238886").
    /// </summary>
    public string ApiVersion { get; set; } = "v20.0";
}
