using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IExternalServices;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Contracts.Response;
using BankingSystem.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace BankingSystem.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExchangeRateService _exchangeRateService;
        private readonly IConfiguration _configuration;

        public AccountService(IUnitOfWork unitOfWork, IExchangeRateService exchangeRateService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _exchangeRateService = exchangeRateService;
            _configuration = configuration;
        }

        //tatia
        public async Task<Response<Account>> CreateAccountAsync(CreateAccountDTO createAccountDto)
        {
            var response = new Response<Account>();
            int personId = await _unitOfWork.PersonRepository.FindIdByIDNumberAsync(createAccountDto.IDNumber);
            if (personId <= 0)
            {
                return response.Set(false, "Such person doesn't exist in our system!");
            }

            bool IBANExists = await _unitOfWork.AccountRepository.IBANExists(createAccountDto.IBAN);
            if (IBANExists)
            {
                return response.Set(false, "Such IBAN already exist in our system!");
            }
            //int currencyId = await _unitOfWork.CurrencyRepository.FindIdByTypeAsync(createAccountDto.Currency.ToString());
            //if (currencyId <= 0)
            //{
            //    return (false, "Such Currency does not exist in our system!", null);
            //}
            var account = new Account
            {
                PersonId = personId,
                IBAN = createAccountDto.IBAN,
                Amount = createAccountDto.Amount,
                Currency = createAccountDto.Currency.ToString()
            };

            int insertedId = await _unitOfWork.AccountRepository.CreateAccountAsync(account);
            if (insertedId <= 0)
            {
                return response.Set(false, "Account could not be created, something happened!");
            }
            account.Id = insertedId;

            //_unitOfWork.SaveChanges();
            return response.Set(true, "Account was created successfully!", account);
        }

        //tamar
        public async Task<Response<List<SeeAccountsDTO>>> SeeAccountsAsync(string email)
        {
            var response = new Response<List<SeeAccountsDTO>>();
            bool accountsExist = await _unitOfWork.AccountRepository.AccountExistForEmail(email);

            if (!accountsExist)
            {
                return response.Set(false, "You don't have any accounts!");
            }

            var accounts = await _unitOfWork.AccountRepository.SeeAccountsByEmail(email);

            if (accounts == null || accounts.Count == 0)
            {
                return response.Set(false, "No accounts found!");
            }

            return response.Set(true, "Accounts retrieved successfully!", accounts);
        }

        //tamar
        public async Task<SimpleResponse> DeleteAccountAsync(string iban)
        {
            var response = new SimpleResponse();
            bool exists = await _unitOfWork.AccountRepository.IBANExists(iban);
            if (!exists)
            {
                return response.Set(false, "Account not found.");
            }

            var balance = await _unitOfWork.AccountRepository.GetBalanceByIBANAsync(iban);
            if (balance > 0)
            {
                return response.Set(false, "Account cannot be deleted while it has a balance.");
            }

            bool deleted = await _unitOfWork.AccountRepository.DeleteAccountByIBANAsync(iban);
            if (!deleted)
            {
                return response.Set(false, "Failed to delete account.");
            }

            //_unitOfWork.SaveChanges();
            return response.Set(true, "Account deleted successfully.");
        }

        #region transactionHelpers
        //for transaction changes
        //tatia
        public async Task<Response<TransferAccountsDTO>> ValidateAccountsForOnlineTransferAsync(string fromIBAN,
            string toIBAN, string email, bool isSelfTransfer)
        {
            var response = new Response<TransferAccountsDTO>();
            if (fromIBAN == toIBAN)
            {
                return response.Set(false, "You can't transfer to same account!");
            }
            var fromToAccounts = new TransferAccountsDTO();
            fromToAccounts.From = await _unitOfWork.AccountRepository.FindAccountByIBANandEmailAsync(fromIBAN, email);
            Account toAccount;
            if (isSelfTransfer)
            {
                fromToAccounts.To = await _unitOfWork.AccountRepository.FindAccountByIBANandEmailAsync(toIBAN, email);
            }
            else
            {
                fromToAccounts.To = await _unitOfWork.AccountRepository.FindAccountByIBANAsync(toIBAN);
            }
            if (fromToAccounts.From is null || fromToAccounts.To is null)
            {
                return response.Set(false, "There is no account for one or both provided IBANs, check well!");
            }

            //if (fromAccount.Amount < amountToTransfer)
            //{
            //    return (false, "You don't have enough money to transfer on your account!", null, null);
            //}
            return response.Set(true, "Accounts validated!", fromToAccounts);
        }
        //tatia
        public async Task<SimpleResponse> UpdateAccountsAmountAsync(int fromAccountId, int toAccountId,
            decimal amountFromAccount, decimal amountToAccount)
        {
            var response = new SimpleResponse();
            _unitOfWork.BeginTransaction();
            _unitOfWork.AccountRepository.SetTransaction(_unitOfWork.Transaction());
            var fromAccountUpdated = await _unitOfWork.AccountRepository.UpdateAccountAmountAsync(fromAccountId, -amountFromAccount);
            var toAccountUpdated = await _unitOfWork.AccountRepository.UpdateAccountAmountAsync(toAccountId, amountToAccount);

            if (!fromAccountUpdated || !toAccountUpdated)
            {
                return response.Set(false, "Balance couldn't be updated!");
            }
            return response.Set(true,"Balance updated successfully!");
        }

        public async Task<SimpleResponse> UpdateBalanceForATMAsync(int accountId, decimal amountToDeduct)
        {
            var response = new SimpleResponse();
            _unitOfWork.BeginTransaction();
            _unitOfWork.AccountRepository.SetTransaction(_unitOfWork.Transaction());
            bool isBalanceUpdated = await _unitOfWork.AccountRepository.UpdateAccountAmountAsync(accountId, -amountToDeduct);

            if (!isBalanceUpdated)
            {
                return response.Set(false, "Failed to update account balance.");
            }

            return response.Set(true, "Balance updated successfully.");
        }
        #endregion
    }
}
