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
        Task<bool> IBANExists(string IBAN);
        Task<bool> AccountExistForEmail(string email);
        Task<int> CreateAccountAsync(Account account);

        Task<List<(int,string)>> SeeAccountsByEmail(string email);  

        
    }
}
