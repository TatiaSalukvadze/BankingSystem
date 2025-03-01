using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Domain.Entities;

namespace BankingSystem.Application.FacadeServices
{
    public class TransactionOperationService : ITransactionOperationService
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
        public async Task<(bool Success, string Message)> OnlineTransactionAsync(CreateOnlineTransactionDTO createTransactionDto,
            string email, bool isSelfTransfer)
        {

            if (createTransactionDto.Amount <= 0)
            {
                return (false, "You need to enter more than 0 value!");
            }
            var (validated, message, fromAccount, toAccount) = await _accountService.ValidateAccountsForOnlineTransferAsync(createTransactionDto.FromIBAN,
                createTransactionDto.ToIBAN, email, isSelfTransfer);
            if (!validated) return (validated, message);


            var (bankProfit, amountFromAccount, amountToAccount) = await _transactionDetailsService.CalculateTransactionAmountAsync(fromAccount.Currency,
                toAccount.Currency, createTransactionDto.Amount, isSelfTransfer);
            if (bankProfit == 0 && amountFromAccount == 0 && amountToAccount == 0)
            {
                return (false, "One of the account has incorrect currency!");
            }
            if (fromAccount.Amount < amountFromAccount)
            {
                return (false, "You don't have enough money to transfer on your account!");
            }
            bool accountsUpdated = await _accountService.UpdateAccountsAmountAsync(fromAccount.Id, toAccount.Id, amountFromAccount, amountToAccount);
            if (!accountsUpdated) return (false, "Balance couldn't be updated!");

            return await _transactionDetailsService.CreateTransactionAsync(bankProfit, createTransactionDto.Amount, fromAccount.Id, toAccount.Id,
                fromAccount.Currency);
        }

        public async Task<(bool success, string message)> WithdrawAsync(WithdrawalDTO withdrawalDto)
        {
            var (cardValidated, cardMessage, card) = await _cardService.AuthorizeCardAsync(withdrawalDto.CardNumber, withdrawalDto.PIN);
            if (!cardValidated) return (false, cardMessage);

            if (withdrawalDto.Amount <= 0)
            {
                return (false, "Withdrawal amount must be greater than zero.");
            }

            var (success, message, balance, currency) = await _accountService.CheckBalanceAndWithdrawalLimitAsync(withdrawalDto.CardNumber, withdrawalDto.PIN, withdrawalDto.Amount);
            if (!success) return (false, message);

            var (conversionSuccess, conversionMessage, amount, fee, totalAmountToDeduct) = await _transactionDetailsService.ConvertAndCalculateAsync(withdrawalDto.Amount, withdrawalDto.Currency.ToString(), currency);
            if (!conversionSuccess) return (false, conversionMessage);
            if (balance < totalAmountToDeduct) return (false, "Not enough money.");

            var (isBalanceUpdated, balanceMessage) = await _accountService.UpdateBalanceAsync(card.AccountId, totalAmountToDeduct);
            if (!isBalanceUpdated) return (false, balanceMessage);

            return await _transactionDetailsService.CreateTransactionAsync(fee, amount, card.AccountId, card.AccountId, currency, IsATM:true);
            
        }
    }
}
