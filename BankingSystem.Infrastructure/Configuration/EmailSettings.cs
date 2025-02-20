namespace BankingSystem.Infrastructure.Configuration
{
    public class EmailSettings
    {
        public string? SmtpServer { get; set; }
        public int Port { get; set; }
        public string? SenderEmail { get; set; }
        public string? SenderName { get; set; }
        public string? SmtpUser { get; set; }
        public string? SmtpPassword { get; set; }
    }
}
