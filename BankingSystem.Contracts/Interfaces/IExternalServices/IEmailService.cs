namespace BankingSystem.Contracts.Interfaces.IExternalServices
{
    public interface IEmailService
    {
        Task SendEmail(string email, string subject, string message);
        Task SendTokenEmailAsync(string token, string email, string ClientUrl, string message);
    }
}
