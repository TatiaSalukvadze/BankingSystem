using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Domain.Entities;

namespace BankingSystem.Contracts.Interfaces.IRepositories
{
    public interface IAccountRepository : IRepository
    {
        Task<Account?> FindAccountByIBANAsync(string IBAN);
        Task<Account> FindAccountByIBANandEmailAsync(string IBAN, string email);
        Task<bool> IBANExistsAsync(string IBAN);
        Task<bool> AccountExistForEmailAsync(string email);
        Task<int> AccountsCountForEmailAsync(string email);
        Task<int> CreateAccountAsync(Account account);
        Task<List<SeeAccountsDTO>> SeeAccountsByEmailAsync(string email, int offset, int perPage);
        Task<bool> UpdateAccountAmountAsync(int id,decimal amount);
        Task<bool> DeleteAccountByIBANAsync(string iban);
        Task<decimal> GetBalanceByIBANAsync(string iban);
    }
}
