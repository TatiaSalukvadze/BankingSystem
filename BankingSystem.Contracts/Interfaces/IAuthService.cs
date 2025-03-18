using Microsoft.AspNetCore.Identity;

namespace BankingSystem.Contracts.Interfaces
{
    public interface IAuthService
    {
        string GenerateToken(IdentityUser user, string role);
    }
}
