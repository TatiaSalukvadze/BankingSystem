using BankingSystem.Contracts.DTOs;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IExternalServices;
using BankingSystem.Contracts.Interfaces.IRepositories;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Domain.Entities;

//using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ICurrencyService _currencyService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IAccountRepository _accountRepository;

        public TransactionService(IConfiguration configuration, IUnitOfWork unitOfWork, ICurrencyService currencyService,
            IAccountRepository accountRepository)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _currencyService = currencyService;
            _accountRepository = accountRepository;

        }
        public async Task<(bool Success, string Message, object Data)> OnlineTransactionAsync(CreateTransactionDTO createTransactionDto,
            string email, bool isSelfTransfer)
        {
            try
            {
                if(createTransactionDto.Amount <= 0)
                {
                    return (false, "You need to enter more than 0 value!", null);
                }
                var (validated, message, fromAccount, toAccount) = await ValidateAccountsAsync(createTransactionDto.FromIBAN,
                    createTransactionDto.ToIBAN, email, createTransactionDto.Amount, isSelfTransfer);
                if (!validated) return (validated, message, null);
                

                var (bankProfit, amountFromAccount, amountToAccount) = await CalculateTransactionAmountAsync(fromAccount.CurrencyId,
                    toAccount.CurrencyId, createTransactionDto.Amount, isSelfTransfer);
                bool accountsUpdated = await UpdateAccountsAmountAsync(fromAccount.Id, toAccount.Id, amountFromAccount, amountToAccount);
                if(!accountsUpdated) return (false, "Balance couldn't be updated!", null);

                var transaction = new TransactionDetails
                {
                    BankProfit = bankProfit,
                    Amount = createTransactionDto.Amount,
                    FromAccountId = fromAccount.Id,
                    ToAccountId = toAccount.Id,
                    CurrencyId = fromAccount.CurrencyId,
                    IsATM = false,
                };
              
                int insertedId = await _unitOfWork.TransactionDetailsRepository.CreateTransactionAsync(transaction);
                if (insertedId <= 0)
                {
                    return (false, "Transaction could not be created, something happened!", null);
                }

                transaction.Id = insertedId;
                _unitOfWork.SaveChanges();
                return (true, "Transaction was successfull!", transaction);
            }
            catch (Exception ex) { return (false, ex.Message, null); }

        }
        //helper methods
        private async Task<(bool Validated, string Message, Account from, Account to)> ValidateAccountsAsync(string fromIBAN,
            string toIBAN, string email,decimal amountToTransfer, bool isSelfTransfer)
        {
            var fromAccount = await _accountRepository.FindAccountByIBANandEmailAsync(fromIBAN, email);
            Account toAccount;
            if (isSelfTransfer)
            {
                toAccount = await _accountRepository.FindAccountByIBANandEmailAsync(toIBAN, email);
            }
            else
            {
                toAccount = await _accountRepository.FindAccountByIBANAsync(toIBAN);
            }
            if (fromAccount is null || toAccount is null)
            {
                return (false, "There is no account for one or both provided IBANs, check well!", null, null);
            }

            if (fromAccount.Amount < amountToTransfer)
            {
                return (false, "You don't have enough money to transfer on your account!", null, null);
            }
            return (true, "Accounts validated!", fromAccount, toAccount);
        }

        private async Task<(decimal bankProfit, decimal amountFromAccount, decimal amountToAccount)> CalculateTransactionAmountAsync(
            int fromCurrencyId, int toCurrencyId, decimal amountToTransfer, bool isSelfTransfer)
        {
            decimal currencyRate = await CalculateCurrencyRateAsync(fromCurrencyId, toCurrencyId);


            decimal fee;
            decimal extraFeeValue = 0;
            if (isSelfTransfer)
            {
                fee = _configuration.GetValue<decimal>("TransactionFees:SelfTransferPercent");
            }
            else
            {
                fee = _configuration.GetValue<decimal>("TransactionFees:StandartTransferPercent");
                extraFeeValue = _configuration.GetValue<decimal>("TransactionFees:StandartTransferValue");
            }

            var bankProfit = amountToTransfer * fee / 100 + extraFeeValue;
            var amountFromAccount = amountToTransfer + bankProfit;
            var amountToAccount = amountToTransfer * currencyRate;
            return(bankProfit, amountFromAccount, amountToAccount);

        }
        private async Task<decimal> CalculateCurrencyRateAsync(int fromCurrencyId, int toCurrencyId)
        {
            decimal currencyRate = 1;
            if (fromCurrencyId != toCurrencyId)
            {
                var fromCurrency = ((CurrencyType)fromCurrencyId).ToString();
                var toCurrency = ((CurrencyType)toCurrencyId).ToString();
                currencyRate = await _currencyService.GetCurrencyRate(fromCurrency, toCurrency);
            }
            return currencyRate;

        }

        private async Task<bool> UpdateAccountsAmountAsync(int fromAccountId, int toAccountId, 
            decimal amountFromAccount, decimal amountToAccount)
        {
            var fromAccountUpdated = await _unitOfWork.AccountRepository.UpdateAccountAmountAsync(fromAccountId, -amountFromAccount);
            var toAccountUpdated = await _unitOfWork.AccountRepository.UpdateAccountAmountAsync(toAccountId, amountToAccount);

            if (!fromAccountUpdated || !toAccountUpdated)
            {
                return false; 
            }
            return true;

        }

        public async Task<(bool Success, string Message, object Data)> NumberOfTransactionsAsync()
        {
            var transactionCountDTO = await _unitOfWork.TransactionDetailsRepository.NumberOfTransactionsAsync();
            return (true, "Transaction Count retreived!", transactionCountDTO);
        }
    }
}
