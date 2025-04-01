using BankingSystem.Domain.Entities;

namespace BankingSystem.Contracts.Interfaces.IRepositories
{
    public interface IRefreshTokenRepository
    {
        Task<bool> SaveRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken> GetRefreshTokenAsync(string token);
        Task<bool> DeleteRefreshTokenAsync(int id);
        Task<int> DeleteExpiredRefreshTokensAsync();
    }
}
