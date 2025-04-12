using Microsoft.Extensions.Options;
using BankingSystem.Contracts.Interfaces.IExternalServices;
using Microsoft.AspNetCore.WebUtilities;
using BankingSystem.Infrastructure.ExternalServices.Configuration;
using MimeKit;
using MimeKit.Text;
using MailKit.Security;
using MailKit.Net.Smtp;

namespace BankingSystem.Infrastructure.ExternalServices
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly string _templateBasePath;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
            _templateBasePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "BankingSystem.Infrastructure", "ExternalServices", "EmailTemplates");
        }

        public async Task SendEmailAsync(string email, string templatePath, string subject, string message)
        {
            var body = await File.ReadAllTextAsync(templatePath);
            body = body.Replace("{{LinkWithToken}}", message);

            var mail = new MimeMessage()
            {
                From = { new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail) },
                To = { MailboxAddress.Parse(email) },
                Subject = subject,
                Body = new TextPart(TextFormat.Html) { Text = body }
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, SecureSocketOptions.StartTls); 
            await smtp.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPassword); 
            await smtp.SendAsync(mail); 
            await smtp.DisconnectAsync(true); 
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
            await SendEmailAsync(email, templatePath, subject, verificationUrl);
        }
    }
}
