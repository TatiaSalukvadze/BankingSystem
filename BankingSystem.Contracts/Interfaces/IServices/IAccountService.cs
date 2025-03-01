using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Contracts.Interfaces.IServices
{
    public interface IAccountService
    {
        Task<(bool Success, string Message, Account? Data)> CreateAccountAsync(CreateAccountDTO createAccountDto);
        Task<(bool success, string message, List<SeeAccountsDTO>? data)> SeeAccountsAsync(string email);
        Task<(bool success, string message)> DeleteAccountAsync(string iban);
        Task<(bool Validated, string Message, Account from, Account to)> ValidateAccountsForOnlineTransferAsync(string fromIBAN,
            string toIBAN, string email, bool isSelfTransfer);
        Task<bool> UpdateAccountsAmountAsync(int fromAccountId, int toAccountId,
            decimal amountFromAccount, decimal amountToAccount);
        Task<(bool success, string message, decimal balance, string currency)> CheckBalanceAndWithdrawalLimitAsync(string cardNumber, string pin, decimal withdrawalAmount);
        Task<(bool success, string message)> UpdateBalanceAsync(int accountId, decimal amountToDeduct);
    }

}
