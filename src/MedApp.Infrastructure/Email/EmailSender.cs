using MailKit.Net.Smtp;
using MimeKit;
using System.Threading;
using System.Threading.Tasks;

namespace MedApp.Infrastructure.Email;

public static class EmailSender
{
    // Configure your SMTP in appsettings and pass values down in production.
    public static async Task SendAsync(string to, string subject, string body, byte[]? attachment = null, string? attachmentName = null, CancellationToken ct = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("MedApp", "no-reply@medapp.local"));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        var builder = new BodyBuilder { TextBody = body };
        if (attachment != null && !string.IsNullOrWhiteSpace(attachmentName))
        {
            builder.Attachments.Add(attachmentName, attachment);
        }
        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        // For demo: localhost relay or your SMTP
        await client.ConnectAsync("localhost", 25, false, ct);
        // If authentication is required:
        // await client.AuthenticateAsync("username", "password", ct);
        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);
    }
}
