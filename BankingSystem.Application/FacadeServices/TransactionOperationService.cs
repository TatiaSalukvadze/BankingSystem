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

        public async Task<SimpleResponse> OnlineTransactionAsync(CreateOnlineTransactionDTO createTransactionDto,
            string email, bool isSelfTransfer)
        {
            var response = new SimpleResponse();
            //if (createTransactionDto.Amount <= 0)
            //{
            //    return response.Set(false, "You need to enter more than 0 value!", 400);
            //}
            var validateAccountsResponse = await _accountService.ValidateAccountsForOnlineTransferAsync(createTransactionDto.FromIBAN,
                createTransactionDto.ToIBAN, email, isSelfTransfer);
            if (!validateAccountsResponse.Success)
            {
                return response.Set(false, validateAccountsResponse.Message, validateAccountsResponse.StatusCode);
            }

            var transferAccounts = validateAccountsResponse.Data;
            var (fromAccount, toAccount) = (transferAccounts.From, transferAccounts.To);
            var calculationResponse = await _transactionDetailsService.CalculateTransferAmountAsync(fromAccount.Currency,
                toAccount.Currency, createTransactionDto.Amount, isSelfTransfer);
            if (!calculationResponse.Success)
            {
                return response.Set(false, calculationResponse.Message, calculationResponse.StatusCode);
            }

            var transferAmounts = calculationResponse.Data;
            if (fromAccount.Amount < transferAmounts.AmountFromAccount)
            {
                return response.Set(false, "You don't have enough money to transfer on your account!", 400);
            }

            var updateAccountsResponse = await _accountService.UpdateAccountsAmountAsync(fromAccount.Id, toAccount.Id, transferAmounts.AmountFromAccount, transferAmounts.AmountToAccount);
            if (!updateAccountsResponse.Success)
            {
                return updateAccountsResponse;
            }

            return await _transactionDetailsService.CreateTransactionAsync(transferAmounts.BankProfit, createTransactionDto.Amount, 
                fromAccount.Id, toAccount.Id, fromAccount.Currency);
        }

        public async Task<SimpleResponse> WithdrawAsync(WithdrawalDTO withdrawalDto)
        {
            var response = new SimpleResponse(); 

            var validateCardResponse = await _cardService.AuthorizeCardAsync(withdrawalDto.CardNumber, withdrawalDto.PIN);
            if (!validateCardResponse.Success)
            {
                return response.Set(false, validateCardResponse.Message, validateCardResponse.StatusCode);
            }

            //if (withdrawalDto.Amount <= 0)
            //{
            //    return response.Set(false, "Withdrawal amount must be greater than zero.", 400);
            //}

            var card = validateCardResponse.Data;
            var calculationResponse = await _transactionDetailsService.CalculateATMWithdrawalTransactionAsync(card.CardNumber, card.PIN,
                withdrawalDto.Amount, withdrawalDto.Currency.ToString());
            if (!calculationResponse.Success)
            {
                return response.Set(false, calculationResponse.Message, calculationResponse.StatusCode);
            }

            var accountId = card.AccountId;
            var withdrawalData = calculationResponse.Data;
            var updateAccountResponse = await _accountService.UpdateBalanceForATMAsync(accountId, withdrawalData.TotalAmountToDeduct);
            if (!updateAccountResponse.Success)
            {
                return updateAccountResponse;
            }

            return await _transactionDetailsService.CreateTransactionAsync(withdrawalData.BankProfit, withdrawalData.Amount, accountId,
                accountId, withdrawalData.Currency, IsATM:true);
        }
    }
}
