using BankingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Contracts.Interfaces.IRepositories
{
    public interface IRefreshTokenRepository
    {
        Task<bool> SaveRefreshTokenAsync(RefreshToken refreshToken);

        Task<RefreshToken> GetRefreshTokenAsync(string token);
        Task<bool> DeleteRefreshTokensync(int id);
    }
}
