using BankingSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BankingSystem.Contracts.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(string userEmail, string role);
        RefreshToken GenerateRefreshToken(string identityUserId, string deviceId);
        string RenewAccessToken(string oldAcessToken);
    }
}
