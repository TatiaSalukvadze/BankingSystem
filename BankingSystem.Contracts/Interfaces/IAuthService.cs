using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Contracts.Interfaces
{
    public interface IAuthService
    {
        string GenerateToken(IdentityUser user, string role);
    }
}
