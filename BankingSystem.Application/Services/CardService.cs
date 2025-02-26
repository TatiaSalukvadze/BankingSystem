using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IExternalServices;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;

namespace BankingSystem.Application.Services
{
    public class CardService : ICardService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExchangeRateService _exchangeRateService;

        public CardService(IConfiguration configuration,IUnitOfWork unitOfWork, IExchangeRateService exchangeRateService)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _exchangeRateService = exchangeRateService; 
        }
        //tamar
        public async Task<(bool success, string message, Card? data)> CreateCardAsync(CreateCardDTO createCardDto)
        {
            var account = await _unitOfWork.AccountRepository.FindAccountByIBANAsync(createCardDto.IBAN);
            if (account == null)
            {
                return (false, "Account does not exist in the system!", null);
            }

            bool cardNumberExists = await _unitOfWork.CardRepository.CardNumberExistsAsync(createCardDto.CardNumber);
            if (cardNumberExists)
            {
                return (false, "Card number already exists!", null);
            }

            var card = new Card
            {
                AccountId = account.Id,
                CardNumber = createCardDto.CardNumber,
                ExpirationDate = createCardDto.ExpirationDate,
                CVV = createCardDto.CVV,
                PIN = createCardDto.PIN
            };

            int insertedId = await _unitOfWork.CardRepository.CreateCardAsync(card);
            if (insertedId <= 0)
            {
                return (false, "Card could not be created, something went wrong!", null);
            }

            card.Id = insertedId;
            _unitOfWork.SaveChanges();

            return (true, "Card was created successfully!", card);

        }
        //tatia
        public async Task<(bool success, string message, List<CardWithIBANDTO> data)> SeeCardsAsync(string email)
        {
            bool accountsExist = await _unitOfWork.AccountRepository.AccountExistForEmail(email);
            if (!accountsExist)
            {
                return (true, "You don't have accounts!", null);
            }

            var cards = await _unitOfWork.CardRepository.GetCardsForPersonAsync(email);
            if (cards == null || cards.Count == 0)
            {
                return (true, "You don't have cards!", null);
            }

            return (true, "Cards For Account (IBAN) were found!", cards);          

        }
        //tamar
        public async Task<(bool success, string message, SeeBalanceDTO data)> SeeBalanceAsync(string cardNumber, string pin)
        {
            var (cardValidated, message, card) = await CheckCardAsync(cardNumber, pin);
            if (!cardValidated)
            {
                return (false, message, null);
            }

            var balance = await _unitOfWork.CardRepository.GetBalanceAsync(cardNumber, pin);

            if (balance is null || balance.Amount == 0 || balance.Currency == 0)
            {
                return (false, "Unable to retrieve balance.", null);
            }
         
            return (true, "Balance retrieved successfully.", balance);
        }
        //tamar
        public async Task<(bool success, string message)> WithdrawAsync(WithdrawalDTO withdrawalDto)
        {
            var (cardValidated, message, card) = await CheckCardAsync(withdrawalDto.CardNumber, withdrawalDto.PIN);
            if (!cardValidated) return (false, message);

            var (isBalanceValid, balanceMessage, balance) = await GetAccountBalanceAsync(withdrawalDto.CardNumber, withdrawalDto.PIN);
            if (!isBalanceValid) return (false, balanceMessage);

            var (withinLimit, limitMessage) = await IsWithinDailyLimitAsync(card.AccountId, withdrawalDto.Amount);
            if (!withinLimit) return (false, limitMessage);

            var (convertedAmount, conversionMessage) = await ConvertCurrencyIfNeededAsync(withdrawalDto.Amount, withdrawalDto.Currency, balance.Currency);
            if (conversionMessage != null) return (false, conversionMessage);

            decimal fee = CalculateWithdrawalFee(convertedAmount);
            decimal totalAmountToDeduct = convertedAmount + fee;

            if (balance.Amount < totalAmountToDeduct) return (false, "Not enough money.");

            bool isBalanceUpdated = await _unitOfWork.CardRepository.UpdateAccountBalanceAsync(card.AccountId, totalAmountToDeduct);
            if (!isBalanceUpdated) return (false, "Failed to update account balance.");

            var (isTransactionCreated, transactionMessage) = await CreateTransactionAsync(card.AccountId, convertedAmount, fee, withdrawalDto.Currency);
            if (!isTransactionCreated) return (false, transactionMessage);

            return (true, "Withdrawal successful.");
        }

        //tamar
        private async Task<(bool, string, SeeBalanceDTO balance)> GetAccountBalanceAsync(string cardNumber, string pin)
        {
            var balance = await _unitOfWork.CardRepository.GetBalanceAsync(cardNumber, pin);
            if (balance == null || balance.Currency == 0 || balance.Amount == 0)
            {
                return (false, "Unable to retrieve account balance.", null);
            }

            return (true, "", balance);
        }

        //tamar
        private async Task<(bool, string)> IsWithinDailyLimitAsync(int accountId, decimal amount)
        {
            if (amount <= 0)
            {
                return (false, "Withdrawal amount must be greater than zero.");
            }

            decimal withdrawnAmount = await _unitOfWork.TransactionDetailsRepository.GetTotalWithdrawnAmountIn24Hours(accountId);
            decimal newTotal = withdrawnAmount + amount;

            if (newTotal > 10000)
            {
                return (false, "You can't withdraw more than 10000 within 24 hours.");
            }

            return (true, "");
        }

        //tamar
        private async Task<(decimal, string)> ConvertCurrencyIfNeededAsync(decimal amount, CurrencyType fromCurrency, CurrencyType toCurrency)
        {
            if (fromCurrency == toCurrency)
            {
                return (amount, null);
            }

            decimal exchangeRate = await _exchangeRateService.GetCurrencyRateAsync(fromCurrency.ToString(), toCurrency.ToString());

            if (exchangeRate <= 0)
            {
                return (0, "Currency conversion failed.");
            }

            decimal convertedAmount = amount * exchangeRate;
            return (convertedAmount, null);
        }

        //tamar
        private decimal CalculateWithdrawalFee(decimal amount)
        {
            decimal atmWithdrawalPercent = _configuration.GetValue<decimal>("TransactionFees:AtmWithdrawalPercent");
            var withdrawalFee = amount * (atmWithdrawalPercent / 100);
            return withdrawalFee;
        }

        //tamar
        private async Task<(bool, string)> CreateTransactionAsync(int accountId, decimal amount, decimal fee, CurrencyType currency)
        {
            int currencyId = await _unitOfWork.CurrencyRepository.FindIdByTypeAsync(currency.ToString());
            if (currencyId <= 0)
            {
                return (false, "Currency does not exist in our system.");
            }

            var transaction = new TransactionDetails
            {
                BankProfit = fee,
                Amount = amount,
                FromAccountId = accountId,
                ToAccountId = accountId,
                CurrencyId = currencyId,
                IsATM = true
            };

            int insertedId = await _unitOfWork.TransactionDetailsRepository.CreateTransactionAsync(transaction);
            if (insertedId <= 0)
            {
                return (false, "Transaction could not be created, something happened!");
            }

            transaction.Id = insertedId;
            _unitOfWork.SaveChanges();

            return (true, "");
        }



        //tatia
        public async Task<(bool success, string message)> ChangeCardPINAsync([FromForm] ChangeCardPINDTO changeCardDtp)
        {
            var (cardValidated, message, card) = await CheckCardAsync(changeCardDtp.CardNumber, changeCardDtp.OldPIN);
            if (!cardValidated)
            {
                return (false, message);
            }

            bool updated = await _unitOfWork.CardRepository.UpdateCardAsync(card.Id, changeCardDtp.NewPIN);
            if (!updated)
            {
                return (false, "Card PIN could not be updated!");
            }
            _unitOfWork.SaveChanges();
            return (true, $"Card PIN was updated Successfully! New PIN: {changeCardDtp.NewPIN}");
        }
        //both
        private async Task<(bool success, string message, Card card)> CheckCardAsync(string CardNumber, string PIN)
        {
            Card card = await _unitOfWork.CardRepository.GetCardAsync(CardNumber);

            if (card is null)
            {
                return (false, "Card was not found!",null);
            }
            if (card.PIN != PIN)
            {
                return (false, "Incorrect PIN!", null);
            }
            if (CheckCardExpired(card.ExpirationDate))
            {
                return (false, "Card is expired!", null);
            }
            return (true, "Card validated", card);
        }
        //tatia
        private bool CheckCardExpired(string expirationDate)
        {
            var cardDate = expirationDate.Split('/');
            var cardMonth = int.Parse(cardDate[0]);
            var cardYear = int.Parse(cardDate[1]);
            var monthNow = DateTime.Now.Month;
            var yearNow = DateTime.Now.Year % 100;

            if (yearNow < cardYear)
            {
                return false;
            }
            else if (yearNow == cardYear && monthNow > cardMonth)
            {
                return false;
            }
            return true;
        }

        public async Task<(bool success, string message)> CancelCardAsync(string cardNumber)
        {
            var cardExists = await _unitOfWork.CardRepository.CardNumberExistsAsync(cardNumber);
            if (!cardExists)
            {
                return (false, "There is no Card for that Card Number!");
            }

            var cardDeleted = await _unitOfWork.CardRepository.DeleteCardAsync(cardNumber);
            if (!cardDeleted)
            {
                return (false, "Card could not be canceled!");
            }
            _unitOfWork.SaveChanges();
            return (true, "Card was successfully canceled!");
        }
    }
}
