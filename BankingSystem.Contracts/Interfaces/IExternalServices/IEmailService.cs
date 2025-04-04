﻿namespace BankingSystem.Contracts.Interfaces.IExternalServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string templatePath, string subject, string message);
        Task SendTokenEmailAsync(string token, string email, string ClientUrl, string message);
    }
}
