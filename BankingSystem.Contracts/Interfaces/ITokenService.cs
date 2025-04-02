using BankingSystem.Domain.Entities;

namespace BankingSystem.Contracts.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(string userEmail, string role);
        RefreshToken GenerateRefreshToken(string identityUserId, string deviceId);
        string RenewAccessToken(string oldAccessToken);
    }
}
