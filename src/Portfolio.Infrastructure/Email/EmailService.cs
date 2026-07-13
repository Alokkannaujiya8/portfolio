using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Portfolio.Application.Interfaces.Services;

namespace Portfolio.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
        var smtpSettings = _configuration.GetSection("SmtpSettings");
        var displayName = smtpSettings["DisplayName"] ?? "Portfolio Admin";
        var fromEmail = smtpSettings["From"];
        var host = smtpSettings["Host"];
        var portStr = smtpSettings["Port"];
        var userName = smtpSettings["UserName"];
        var password = smtpSettings["Password"];
        var enableSsl = bool.Parse(smtpSettings["EnableSsl"] ?? "false");
        var useStartTls = bool.Parse(smtpSettings["UseStartTls"] ?? "true");

        if (string.IsNullOrEmpty(fromEmail) || string.IsNullOrEmpty(host))
        {
            _logger.LogWarning("SMTP Settings are not fully configured. Email to {ToEmail} was not sent.", toEmail);
            return;
        }

        if (!int.TryParse(portStr, out int port))
        {
            port = 587;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(displayName, fromEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        try
        {
            SecureSocketOptions options = enableSsl ? SecureSocketOptions.SslOnConnect : (useStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
            await client.ConnectAsync(host, port, options);

            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                await client.AuthenticateAsync(userName, password);
            }

            await client.SendAsync(message);
            _logger.LogInformation("Email sent successfully to {ToEmail}.", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {ToEmail}.", toEmail);
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}
