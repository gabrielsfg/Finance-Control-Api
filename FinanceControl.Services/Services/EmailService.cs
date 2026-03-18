using FinanceControl.Domain.Interfaces.Service;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace FinanceControl.Services.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            var smtp = _configuration.GetSection("Smtp");

            using var client = new SmtpClient(smtp["Host"], int.Parse(smtp["Port"]!))
            {
                Credentials = new NetworkCredential(smtp["Username"], smtp["Password"]),
                EnableSsl = bool.Parse(smtp["EnableSsl"] ?? "true")
            };

            var from = smtp["From"]!;

            var message = new MailMessage(from, toEmail)
            {
                Subject = "Reset your password",
                Body = $"""
                    <p>You requested a password reset.</p>
                    <p>Click the link below to set a new password. This link expires in 1 hour.</p>
                    <p><a href="{resetLink}">Reset password</a></p>
                    <p>If you did not request this, ignore this email.</p>
                    """,
                IsBodyHtml = true
            };

            await client.SendMailAsync(message);
        }

        public async Task SendVerificationEmailAsync(string toEmail, string verificationLink)
        {
            var smtp = _configuration.GetSection("Smtp");

            using var client = new SmtpClient(smtp["Host"], int.Parse(smtp["Port"]!))
            {
                Credentials = new NetworkCredential(smtp["Username"], smtp["Password"]),
                EnableSsl = bool.Parse(smtp["EnableSsl"] ?? "true")
            };

            var from = smtp["From"]!;

            var message = new MailMessage(from, toEmail)
            {
                Subject = "Verify your email address",
                Body = $"""
                    <p>Thank you for registering!</p>
                    <p>Click the link below to verify your email address. This link expires in 24 hours.</p>
                    <p><a href="{verificationLink}">Verify email</a></p>
                    <p>If you did not create an account, ignore this email.</p>
                    """,
                IsBodyHtml = true
            };

            await client.SendMailAsync(message);
        }
    }
}
