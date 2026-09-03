using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using TAO.Application.Common.Interfaces;

namespace TAO.Application.Email;

internal sealed class EmailSender : IEmailSender
{
    private readonly EmailOptions _options;

    public EmailSender(
        IOptions<EmailOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendAsync(
        string recipientEmail,
        string subject,
        string body,
        CancellationToken cancellationToken)
    {
        using var client = new SmtpClient(
            _options.Host,
            _options.Port)
        {
            EnableSsl = _options.EnableSsl,
            Credentials = new NetworkCredential(
                _options.Username,
                _options.Password)
        };

        using var message = new MailMessage
        {
            From = new MailAddress(
                _options.FromEmail,
                _options.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };

        message.To.Add(recipientEmail);

        cancellationToken.ThrowIfCancellationRequested();

        await client.SendMailAsync(
            message,
            cancellationToken);
    }
}