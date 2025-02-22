using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Contracts.Interfaces.IRepositories
{
    public interface ICurrencyRepository
    {
        Task<int> FindIdByTypeAsync(string type);
        Task<string> FindTypeByIdAsync(int id);
    }
}
