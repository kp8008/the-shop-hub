using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ECommerceAPI.Services
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587;
        public bool UseSsl { get; set; } = false;
        public string FromEmail { get; set; } = "";
        public string FromName { get; set; } = "The Shop Hub";
        public string Password { get; set; } = "";
    }

    public class GmailEmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public GmailEmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings?.Value ?? new EmailSettings();
        }

        public async Task SendPasswordResetOtpAsync(string toEmail, string otp, CancellationToken cancellationToken = default)
        {
            var subject = "Password Reset OTP - The Shop Hub";
            var body = $"Your OTP for password reset is {otp}. Valid for 10 minutes. Do not share with anyone.";
            await SendAsync(toEmail, subject, body, cancellationToken);
        }

        public async Task SendLoginThankYouAsync(string toEmail, string customerName, CancellationToken cancellationToken = default)
        {
            var name = string.IsNullOrWhiteSpace(customerName) ? "there" : customerName.Trim();
            var subject = "Thank you for logging in - The Shop Hub";
            var dateStr = DateTime.UtcNow.ToString("dd MMM yyyy");
            var body = $"Thank you for login \"{name}\", have a great day, keep shopping with us !!{Environment.NewLine}{Environment.NewLine}Date: {dateStr}";
            await SendAsync(toEmail, subject, body, cancellationToken);
        }

        private async Task SendAsync(string toEmail, string subject, string body, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_settings.FromEmail) || string.IsNullOrWhiteSpace(_settings.Password))
                return; // Skip if not configured (e.g. dev without Gmail)

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail.Trim()));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls, cancellationToken);
            await client.AuthenticateAsync(_settings.FromEmail, _settings.Password, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
    }
}
