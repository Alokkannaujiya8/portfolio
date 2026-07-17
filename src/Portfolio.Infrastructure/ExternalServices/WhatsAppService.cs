using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Portfolio.Application.Common.Configurations;
using Portfolio.Application.Interfaces.Services;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.ExternalServices;

/// <summary>
/// Implementation of IWhatsAppService to send messages via WhatsApp using HttpClientFactory.
/// Supports both Meta Cloud API and Twilio API.
/// </summary>
public class WhatsAppService : IWhatsAppService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly WhatsAppSettings _settings;
    private readonly ILogger<WhatsAppService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="WhatsAppService"/> class.
    /// </summary>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    /// <param name="settings">The WhatsApp configurations.</param>
    /// <param name="logger">The logger instance.</param>
    public WhatsAppService(
        IHttpClientFactory httpClientFactory,
        IOptions<WhatsAppSettings> settings,
        ILogger<WhatsAppService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _settings = settings.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<bool> SendContactInquiryNotificationAsync(ContactMessage contactMessage, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_settings.Provider))
        {
            _logger.LogWarning("WhatsApp notification provider is not configured. Skipping WhatsApp notification.");
            return false;
        }

        string messageText = FormatWhatsAppMessage(contactMessage);

        try
        {
            if (_settings.Provider.Equals("Meta", StringComparison.OrdinalIgnoreCase))
            {
                return await SendViaMetaAsync(messageText, cancellationToken);
            }
            else if (_settings.Provider.Equals("Twilio", StringComparison.OrdinalIgnoreCase))
            {
                return await SendViaTwilioAsync(messageText, cancellationToken);
            }
            else
            {
                _logger.LogError("Unsupported WhatsApp provider configured: '{Provider}'", _settings.Provider);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending WhatsApp notification via '{Provider}'", _settings.Provider);
            return false;
        }
    }

    private async Task<bool> SendViaMetaAsync(string messageText, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();
        
        string apiVersion = string.IsNullOrEmpty(_settings.ApiVersion) ? "v20.0" : _settings.ApiVersion;
        string url = $"https://graph.facebook.com/{apiVersion}/{_settings.PhoneNumberId}/messages";

        var payload = new
        {
            messaging_product = "whatsapp",
            recipient_type = "individual",
            to = _settings.RecipientNumber,
            type = "text",
            text = new
            {
                preview_url = false,
                body = messageText
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.AccessToken);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        _logger.LogInformation("Sending WhatsApp notification via Meta Cloud API to {Recipient}", _settings.RecipientNumber);

        var response = await client.SendAsync(request, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("WhatsApp notification sent successfully via Meta Cloud API. Response: {Response}", responseContent);
            return true;
        }
        else
        {
            _logger.LogError("Failed to send WhatsApp notification via Meta Cloud API. Status Code: {StatusCode}. Response: {Response}", 
                response.StatusCode, responseContent);
            return false;
        }
    }

    private async Task<bool> SendViaTwilioAsync(string messageText, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();
        
        string url = $"https://api.twilio.com/2010-04-01/Accounts/{_settings.PhoneNumberId}/Messages.json";

        var authBytes = Encoding.UTF8.GetBytes($"{_settings.PhoneNumberId}:{_settings.AccessToken}");
        string authHeaderVal = Convert.ToBase64String(authBytes);

        var formData = new Dictionary<string, string>
        {
            { "To", $"whatsapp:{_settings.RecipientNumber}" },
            { "From", $"whatsapp:{_settings.ApiVersion}" }, // For Twilio, ApiVersion configuration stores the Sender Number
            { "Body", messageText }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new FormUrlEncodedContent(formData)
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeaderVal);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        _logger.LogInformation("Sending WhatsApp notification via Twilio API to {Recipient}", _settings.RecipientNumber);

        var response = await client.SendAsync(request, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("WhatsApp notification sent successfully via Twilio API. Response: {Response}", responseContent);
            return true;
        }
        else
        {
            _logger.LogError("Failed to send WhatsApp notification via Twilio API. Status Code: {StatusCode}. Response: {Response}", 
                response.StatusCode, responseContent);
            return false;
        }
    }

    private string FormatWhatsAppMessage(ContactMessage msg)
    {
        string company = ExtractValue(msg.Message, "Company");
        string phone = ExtractValue(msg.Message, "Phone");

        // Format message matching requested structure
        var sb = new StringBuilder();
        sb.AppendLine("📩 *New Portfolio Inquiry*");
        sb.AppendLine();
        sb.AppendLine($"*Name:* {msg.Name}");
        sb.AppendLine($"*Company:* {company}");
        sb.AppendLine($"*Email:* {msg.Email}");
        sb.AppendLine($"*Phone:* {phone}");
        sb.AppendLine($"*Subject:* {msg.Subject}");
        sb.AppendLine($"*Message:* {msg.Message}");
        sb.AppendLine();
        sb.AppendLine($"*Submitted At:* {msg.CreatedAt.ToLocalTime():yyyy-MM-dd HH:mm:ss}");

        return sb.ToString();
    }

    private string ExtractValue(string message, string label)
    {
        if (string.IsNullOrEmpty(message)) return "N/A";
        
        var lines = message.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            if (line.StartsWith(label + ":", StringComparison.OrdinalIgnoreCase))
            {
                var parts = line.Split(':', 2);
                if (parts.Length == 2)
                {
                    return parts[1].Trim();
                }
            }
        }
        return "N/A";
    }
}
