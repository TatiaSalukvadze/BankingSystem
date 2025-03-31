using BankingSystem.Contracts.Interfaces.IServices;
using Microsoft.Extensions.Configuration;

namespace BankingSystem.Application.HelperServices
{
    public class HashingService : IHashingService
    {
        private readonly int _workfactory;

        public HashingService(IConfiguration configuration)
        {
            _workfactory = configuration.GetValue<int>("BCryptSettings:WorkFactor");
        }

        public string HashValue(string value)
        {
            var hashedValue = BCrypt.Net.BCrypt.EnhancedHashPassword(value, _workfactory);
            return hashedValue;
        }

        public bool VerifyValue(string value, string hash)
        {
            var verified = BCrypt.Net.BCrypt.EnhancedVerify(value, hash);
            return verified;
        }
    }
}
