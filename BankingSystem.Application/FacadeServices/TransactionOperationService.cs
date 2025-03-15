using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Contracts.Response;

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
        public async Task<SimpleResponse> OnlineTransactionAsync(CreateOnlineTransactionDTO createTransactionDto,
            string email, bool isSelfTransfer)
        {
            var response = new SimpleResponse();
            if (createTransactionDto.Amount <= 0)
            {
                return response.Set(false, "You need to enter more than 0 value!");
            }
            var (validated, validationMessage, transferAccounts) = await _accountService.ValidateAccountsForOnlineTransferAsync(createTransactionDto.FromIBAN,
                createTransactionDto.ToIBAN, email, isSelfTransfer);
            if (!validated)
            {
                return response.Set(validated, validationMessage);
            }
            var (fromAccount, toAccount) = (transferAccounts.From, transferAccounts.To);
            var (success, message, transferAmounts) = await _transactionDetailsService.CalculateTransferAmountAsync(fromAccount.Currency,
                toAccount.Currency, createTransactionDto.Amount, isSelfTransfer);
            if (!success)
            {
                return response.Set(success, message);
            }
            if (fromAccount.Amount < transferAmounts.AmountFromAccount)
            {
                return response.Set(false, "You don't have enough money to transfer on your account!");
            }
            var (accountsUpdated, updateMessage) = await _accountService.UpdateAccountsAmountAsync(fromAccount.Id, toAccount.Id, transferAmounts.AmountFromAccount, transferAmounts.AmountToAccount);
            if (!accountsUpdated)
            {
                return response.Set(accountsUpdated, updateMessage);
            }

            return await _transactionDetailsService.CreateTransactionAsync(transferAmounts.BankProfit, createTransactionDto.Amount, fromAccount.Id, toAccount.Id,
                fromAccount.Currency);
        }

        public async Task<SimpleResponse> WithdrawAsync(WithdrawalDTO withdrawalDto)
        {
            var response = new SimpleResponse(); 
            var validateCardResponse = await _cardService.AuthorizeCardAsync(withdrawalDto.CardNumber, withdrawalDto.PIN);
            if (!validateCardResponse.Success)
            {
                return response.Set(false, validateCardResponse.Message);
            }
            if (withdrawalDto.Amount <= 0)
            {
                return response.Set(false, "Withdrawal amount must be greater than zero.");
            }
    
            var calculationResponse = await _transactionDetailsService.CalculateATMWithdrawalTransactionAsync(withdrawalDto.CardNumber, withdrawalDto.PIN, withdrawalDto.Amount, withdrawalDto.Currency.ToString());
            if (!calculationResponse.Success)
            {
                return response.Set(false, calculationResponse.Message);
            }

            var card = validateCardResponse.Data;
            var withdrawalData = calculationResponse.Data;
            var updateAccountResponse = await _accountService.UpdateBalanceForATMAsync(card.AccountId, withdrawalData.TotalAmountToDeduct);
            if (!updateAccountResponse.Success)
            {
                return response.Set(false, updateAccountResponse.Message);
            }

            return await _transactionDetailsService.CreateTransactionAsync(withdrawalData.Fee, withdrawalData.Balance, card.AccountId, card.AccountId, withdrawalData.Currency, IsATM:true);
        }
    }
}
