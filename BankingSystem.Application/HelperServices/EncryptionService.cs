using BankingSystem.Contracts.Interfaces.IServices;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace BankingSystem.Application.HelperServices
{
    public class EncryptionService : IEncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _IV;

        public EncryptionService(IConfiguration configuration)
        {
            _key = Encoding.ASCII.GetBytes(configuration["EncryptionSettings:Key"]);
            _IV = Encoding.ASCII.GetBytes(configuration["EncryptionSettings:IV"]);
        }

        public string Encrypt(string value)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _IV;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (var sw = new StreamWriter(cs))
                        {
                            sw.Write(value);
                        }
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public string Decrypt(string value)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _IV;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (var ms = new MemoryStream(Convert.FromBase64String(value)))
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}

 