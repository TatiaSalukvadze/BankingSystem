namespace BankingSystem.Contracts.Interfaces.IServices
{
    public interface IEncryptionService
    {
        string Encrypt(string value);
        string Decrypt(string value);
    }
}
