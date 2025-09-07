// Infrastructure/Email/EmailSender.cs
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace MedApp.Infrastructure.Email;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string body,
                   byte[]? attachment = null, string? attachmentName = null,
                   CancellationToken ct = default);
}

public class EmailSender : IEmailSender
{
    private readonly EmailSettings _cfg;

    public EmailSender(IOptions<EmailSettings> options)
    {
        _cfg = options.Value;
    }

    public async Task SendAsync(string to, string subject, string body,
                                byte[]? attachment = null, string? attachmentName = null,
                                CancellationToken ct = default)
    {
        var msg = new MimeMessage();
        msg.From.Add(new MailboxAddress(_cfg.FromName, _cfg.FromAddress));
        msg.To.Add(MailboxAddress.Parse(to));
        msg.Subject = subject;

        var builder = new BodyBuilder { TextBody = body };
        if (attachment is not null && !string.IsNullOrWhiteSpace(attachmentName))
            builder.Attachments.Add(attachmentName!, attachment);

        msg.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        var ssl = _cfg.UseSsl ? SecureSocketOptions.SslOnConnect :
                  _cfg.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.None;

        await client.ConnectAsync(_cfg.Host, _cfg.Port, ssl, ct);

        if (!string.IsNullOrWhiteSpace(_cfg.Username) && !string.IsNullOrWhiteSpace(_cfg.Password))
        {
            await client.AuthenticateAsync(_cfg.Username, _cfg.Password, ct);
        }


        await client.SendAsync(msg, ct);
        await client.DisconnectAsync(true, ct);
    }
}
