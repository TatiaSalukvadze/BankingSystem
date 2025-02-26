using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Domain.Entities;
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
    }
}
