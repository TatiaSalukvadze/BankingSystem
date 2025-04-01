using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using BankingSystem.Contracts.Interfaces.IExternalServices;
using Microsoft.AspNetCore.WebUtilities;
using BankingSystem.Infrastructure.ExternalServices.Configuration;

namespace BankingSystem.Infrastructure.ExternalServices
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private string _templateBasePath;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
            _templateBasePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "BankingSystem.Infrastructure", "ExternalServices", "EmailTemplates");
        }

        public async Task SendEmail(string email, string templatePath, string subject, string message)
        {
            var body = await File.ReadAllTextAsync(templatePath);
            body = body.Replace("{{VerificationLink}}", message);

            var mail = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mail.To.Add(email);
            using var smpt = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
            {
                Credentials = new NetworkCredential(_emailSettings.SmtpUser, _emailSettings.SmtpPassword),
                EnableSsl = true
            };

            await smpt.SendMailAsync(mail);
        }

        public async Task SendTokenEmailAsync(string token, string email, string ClientUrl, string subject)
        {
            var templatePath = Path.Combine(_templateBasePath, "SendTokenTemplate.html");
            var tokenEmail = new Dictionary<string, string?>()
                {
                    {"email", email},
                    {"token", token }
                };
            var verificationUrl = QueryHelpers.AddQueryString(ClientUrl, tokenEmail);
            await SendEmail(email, templatePath, subject, verificationUrl);
        }
    }
}
