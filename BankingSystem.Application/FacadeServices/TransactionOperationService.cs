using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IExternalServices;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Application.FacadeServices
{
    internal class TransactionOperationService
    {
        private readonly IAccountService _accountService;
        private readonly ICardService _cardService;
        private readonly ITransactionDetailsService _transactionDetailsService;


        public TransactionOperationService(IAccountService accountService, ICardService cardService, ITransactionDetailsService transactionDetailsService)
        {
            _accountService = accountService;
            _cardService = cardService;
            _transactionDetailsService = transactionDetailsService;

        }


        //tatia
        public async Task<(bool Success, string Message, TransactionDetails Data)> OnlineTransactionAsync(CreateTransactionDTO createTransactionDto,
            string email, bool isSelfTransfer)
        {

            if (createTransactionDto.Amount <= 0)
            {
                return (false, "You need to enter more than 0 value!", null);
            }
            var (validated, message, fromAccount, toAccount) = await _accountService.ValidateAccountsForOnlineTransferAsync(createTransactionDto.FromIBAN,
                createTransactionDto.ToIBAN, email, isSelfTransfer);
            if (!validated) return (validated, message, null);


            var (bankProfit, amountFromAccount, amountToAccount) = await _transactionDetailsService.CalculateTransactionAmountAsync(fromAccount.CurrencyId,
                toAccount.CurrencyId, createTransactionDto.Amount, isSelfTransfer);
            if (bankProfit == 0 && amountFromAccount == 0 && amountToAccount == 0)
            {
                return (false, "One of the account has incorrect currency!", null);
            }
            if (fromAccount.Amount < amountFromAccount)
            {
                return (false, "You don't have enough money to transfer on your account!", null);
            }
            bool accountsUpdated = await _accountService.UpdateAccountsAmountAsync(fromAccount.Id, toAccount.Id, amountFromAccount, amountToAccount);
            if (!accountsUpdated) return (false, "Balance couldn't be updated!", null);

            return await _transactionDetailsService.CreateTransactionAsync(bankProfit, createTransactionDto.Amount, fromAccount.Id, toAccount.Id,
                fromAccount.CurrencyId);
          


        }
       

    }
}
