namespace BankingSystem.Contracts.Interfaces.IServices
{
    public interface IHashingService
    {
        string HashValue(string value);
        bool VerifyValue(string value, string hash);
    }
}
