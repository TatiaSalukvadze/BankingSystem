﻿using BankingSystem.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using BankingSystem.Contracts.Interfaces.IExternalServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace BankingSystem.Infrastructure.ExternalServices
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailPlaint(string email, string subject, string message)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail!, _emailSettings.SenderName),
                Subject = subject,
                Body = $"<p>{message}</p>",
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
            var tokenEmail = new Dictionary<string, string?>()
                {
                    {"email", email},
                    {"token", token }
                };
            var verificationUrl = QueryHelpers.AddQueryString(ClientUrl!, tokenEmail);
            await SendEmailPlaint(email, subject, verificationUrl);
        }
    }
}
