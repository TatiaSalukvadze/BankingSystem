namespace BankingSystem.Contracts.Interfaces.IExternalServices
{
    public interface IEmailService
    {
        Task SendEmailPlaint(string email, string subject, string message);
    }
}
