using System.Text.Json;
using Microsoft.Extensions.Options;

namespace ECommerceAPI.Services
{
    public class ResendSettings
    {
        public string ApiKey { get; set; } = "";
        public string From { get; set; } = "The Shop Hub <onboarding@resend.dev>";
    }

    public class ResendEmailService : IEmailService
    {
        private readonly ResendSettings _settings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ResendEmailService> _logger;

        public ResendEmailService(
            IOptions<ResendSettings> settings,
            IHttpClientFactory httpClientFactory,
            ILogger<ResendEmailService> logger)
        {
            _settings = settings?.Value ?? new ResendSettings();
            _httpClientFactory = httpClientFactory;
            _logger = logger;
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

        public async Task SendWelcomeAsync(string toEmail, string customerName, CancellationToken cancellationToken = default)
        {
            var name = string.IsNullOrWhiteSpace(customerName) ? "there" : customerName.Trim();
            var subject = "Welcome to The Shop Hub!";
            var body = $"Hi {name},{Environment.NewLine}{Environment.NewLine}Thank you for signing up. You can now shop, add to cart, save wishlist and place orders. Have a great day!{Environment.NewLine}{Environment.NewLine}- The Shop Hub";
            await SendAsync(toEmail, subject, body, cancellationToken);
        }

        private async Task SendAsync(string toEmail, string subject, string body, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_settings.ApiKey))
                throw new InvalidOperationException("Resend not configured. Set Resend:ApiKey (e.g. Render env Resend__ApiKey).");

            _logger.LogInformation("Resend: sending to {To}, subject: {Subject}", toEmail, subject);

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _settings.ApiKey.Trim());
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "TheShopHub-API/1.0");

            var payload = new Dictionary<string, object>
            {
                ["from"] = _settings.From.Trim(),
                ["to"] = new[] { toEmail.Trim() },
                ["subject"] = subject,
                ["text"] = body
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.resend.com/emails", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Resend failed to {To}: {Status} {Body}", toEmail, response.StatusCode, errBody);
                throw new InvalidOperationException($"Resend API error: {response.StatusCode} - {errBody}");
            }

            _logger.LogInformation("Resend email sent successfully to {To}", toEmail);
        }
    }
}
