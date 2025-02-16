using BankingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Contracts.Interfaces.IRepositories
{
    public interface IAccountRepository 
    {
        Task<Account?> FindAccountByIBANAsync(string IBAN);
        Task<bool> IBANExists(string IBAN);
        Task<int> CreateAccountAsync(Account account);
    }
}
