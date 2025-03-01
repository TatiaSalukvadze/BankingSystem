using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Enums;
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
        Task<Account> FindAccountByIBANandEmailAsync(string IBAN, string email);
        Task<bool> IBANExists(string IBAN);
        Task<bool> AccountExistForEmail(string email);
        Task<int> CreateAccountAsync(Account account);
        Task<List<SeeAccountsDTO>> SeeAccountsByEmail(string email);
        Task<bool> UpdateAccountAmountAsync(int id,decimal amount);
        Task<bool> DeleteAccountByIBANAsync(string iban);
        Task<decimal> GetBalanceByIBANAsync(string iban);
        Task<SeeBalanceDTO> GetBalanceAsync(string cardNumber, string pin);
        Task<bool> UpdateAccountBalanceAsync(int accountId, decimal totalAmountToDeduct);
        Task<BalanceAndWithdrawalDTO> GetBalanceAndWithdrawnAmountAsync(string cardNumber, string pin);
    }
}
