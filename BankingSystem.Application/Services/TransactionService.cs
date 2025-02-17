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
        public async Task<(bool Success, string Message, object Data)> SelfTransactionAsync(CreateTransactionDTO createTransactionDto,
            string email)
        {
            try
            {
                var fromAccount = await _accountRepository.FindAccountByIBANandEmailAsync(createTransactionDto.FromIBAN, email);
                var toAccount = await _accountRepository.FindAccountByIBANandEmailAsync(createTransactionDto.ToIBAN, email);
                if (fromAccount is null || toAccount is null)
                {
                    return (false, "You don't have account for one or both of IBANs, check well!", null);
                }

                if (fromAccount.Amount < createTransactionDto.Amount)
                {
                    return (false, "You don't have enough money to transfer on your account!", null);
                }

                decimal currencyRate = 1;
                if (fromAccount.CurrencyId != toAccount.CurrencyId)
                {
                    var fromCurrency = ((Domain.Enums.CurrencyType)fromAccount.CurrencyId).ToString();
                    var toCurrency = ((Domain.Enums.CurrencyType)toAccount.CurrencyId).ToString();
                    currencyRate = await _currencyService.GetCurrencyRate(fromCurrency, toCurrency);
                }

                var fee = _configuration.GetValue<decimal>("TransactionFees:SelfTransfer");

                var bankProfit = createTransactionDto.Amount * fee / 100;         
                var amountFromAccount = createTransactionDto.Amount + bankProfit;
                var amountToAccount = createTransactionDto.Amount * currencyRate;

                var fromAccountedUpdated = await _unitOfWork.AccountRepository.UpdateAccountAmountAsync(fromAccount.Id, -amountFromAccount);
                var toAccountedUpdated = await _unitOfWork.AccountRepository.UpdateAccountAmountAsync(toAccount.Id, amountToAccount);

                if(!fromAccountedUpdated || !toAccountedUpdated)
                {
                    return (false, "Balance couldn't be updated!", null);
                }
                var transaction = new TransactionDetails
                {
                    BankProfit = bankProfit,
                    Amount = createTransactionDto.Amount,
                    FromAccountId = fromAccount.Id,
                    ToAccountId = toAccount.Id,
                    CurrencyId = fromAccount.CurrencyId,
                };
              
                int insertedId = await _unitOfWork.TransactionDetailsRepository.CreateTransactionAsync(transaction);
                if (insertedId <= 0)
                {
                    return (false, "Transaction could not be created, something happened!", null);
                }

                transaction.Id = insertedId;
                _unitOfWork.SaveChanges();
                return (true, "Transaction was successfully!", transaction);
            }
            catch (Exception ex) { return (false, ex.Message, null); }
        }
    }
}
