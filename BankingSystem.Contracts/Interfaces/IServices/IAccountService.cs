using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Contracts.Response;
using BankingSystem.Domain.Entities;

namespace BankingSystem.Contracts.Interfaces.IServices
{
    public interface IAccountService
    {
        Task<Response<Account>> CreateAccountAsync(CreateAccountDTO createAccountDto);
        Task<(bool success, string message, List<SeeAccountsDTO>? data)> SeeAccountsAsync(string email);
        Task<(bool success, string message)> DeleteAccountAsync(string iban);
        Task<(bool Validated, string Message, TransferAccountsDTO Accounts)> ValidateAccountsForOnlineTransferAsync(string fromIBAN,
            string toIBAN, string email, bool isSelfTransfer);
        Task<(bool success, string message)> UpdateAccountsAmountAsync(int fromAccountId, int toAccountId,
            decimal amountFromAccount, decimal amountToAccount);
        Task<(bool success, string message)> UpdateBalanceForATMAsync(int accountId, decimal amountToDeduct);
    }
}
