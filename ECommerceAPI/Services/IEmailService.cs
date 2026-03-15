namespace ECommerceAPI.Services
{
    public interface IEmailService
    {
        Task SendPasswordResetOtpAsync(string toEmail, string otp, CancellationToken cancellationToken = default);
        Task SendLoginThankYouAsync(string toEmail, string customerName, CancellationToken cancellationToken = default);
        Task SendWelcomeAsync(string toEmail, string customerName, CancellationToken cancellationToken = default);
    }
}
