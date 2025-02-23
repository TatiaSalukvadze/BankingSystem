using BankingSystem.Contracts.DTOs;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        //tatia
        public async Task<(bool Success, string Message, Account? Data)> CreateAccountAsync(CreateAccountDTO createAccountDto)
        {

                int personId = await _unitOfWork.PersonRepository.FindIdByIDNumberAsync(createAccountDto.IDNumber);
                if (personId <= 0)
                {
                    return (false, "Such person doesn't exist in our system!", null);
                }

                bool IBANExists = await _unitOfWork.AccountRepository.IBANExists(createAccountDto.IDNumber);
                if (IBANExists)
                {
                    return (false, "Such IBAN already exist in our system!", null);
                }
                int currencyId = await _unitOfWork.CurrencyRepository.FindIdByTypeAsync(createAccountDto.Currency.ToString());
                if (currencyId <= 0)
                {
                    return (false, "Such Currency does not exist in our system!", null);
                }
                var account = new Account
                {
                    PersonId = personId,
                    IBAN = createAccountDto.IBAN,
                    Amount = createAccountDto.Amount,
                    CurrencyId = currencyId//(int)createAccountDto.Currency
                };

            int insertedId = await _unitOfWork.AccountRepository.CreateAccountAsync(account);
            if (insertedId <= 0)
            {
                return (false, "Account could not be created, something happened!", null);
            }
            account.Id = insertedId;

            _unitOfWork.SaveChanges();
            return (true, "Account was created successfully!", account);

        }
        //tamar
        public async Task<(bool success, string message, List<SeeAccountsDTO>? data)> SeeAccountsAsync(string email)
        {
            bool accountsExist = await _unitOfWork.AccountRepository.AccountExistForEmail(email);

            if (!accountsExist)
            {
                return (true, "You don't have any accounts!", null);
            }

            var accounts = await _unitOfWork.AccountRepository.SeeAccountsByEmail(email);

            if (accounts == null || accounts.Count == 0)
            {
                return (true, "No accounts found!", null);
            }

            return (true, "Accounts retrieved successfully!", accounts);
        }

        //tamar
        public async Task<(bool success, string message)> DeleteAccountAsync(string iban)
        {
            bool exists = await _unitOfWork.AccountRepository.IBANExists(iban);
            if (!exists)
            {
                return (false, "Account not found.");
            }

            var balance = await _unitOfWork.AccountRepository.GetBalanceByIBANAsync(iban);
            if (balance > 0)
            {
                return (false, "Account cannot be deleted while it has a balance.");
            }

            bool deleted = await _unitOfWork.AccountRepository.DeleteAccountByIBANAsync(iban);
            if (!deleted)
            {
                return (false, "Failed to delete account.");
            }

            _unitOfWork.SaveChanges();
            return (true, "Account deleted successfully.");
        }
    }
}
