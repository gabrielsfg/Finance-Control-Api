namespace FinanceControl.Domain.Interfaces.Service
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string toEmail, string resetLink);

        Task SendVerificationEmailAsync(string toEmail, string verificationLink);
    }
}
