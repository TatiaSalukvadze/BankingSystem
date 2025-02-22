using BankingSystem.Contracts.DTOs;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IExternalServices;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

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

                bool cardNumberExists = await _unitOfWork.CardRepository.CardNumberExists(createCardDto.CardNumber);
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

            if (balance.Amount == 0 || balance.Currency == null)//?????
            {
                return (false, "Unable to retrieve balance.", null);
            }
         

            return (true, "Balance retrieved successfully.", balance);
        }

        public async Task<(bool success, string message)> WithdrawAsync(WithdrawalDTO withdrawalDto)
        {
            var (cardValidated, message, card) = await CheckCardAsync(withdrawalDto.CardNumber, withdrawalDto.PIN);
            if (!cardValidated)
            {
                return (false, message);
            }

            if (withdrawalDto.Amount <= 0)
            {
                return (false, "Withdrawal amount must be greater than zero.");
            }

            var account = await _unitOfWork.CardRepository.GetBalanceAsync(withdrawalDto.CardNumber, withdrawalDto.PIN);
            if (account.Currency == 0 || account.Amount == null)
            {
                return (false, "Unable to retrieve account balance.");
            }

            decimal withdrawnAmountIn24Hours = await _unitOfWork.TransactionDetailsRepository.GetTotalWithdrawnAmountIn24Hours(card.AccountId);
            if (withdrawnAmountIn24Hours + withdrawalDto.Amount > 10000)
            {
                return (false, "You can't withdraw more than 10000 within 24 hours.");
            }

            if (withdrawalDto.Currency != (CurrencyType)account.Currency)
            {
                decimal currencyRate = await _exchangeRateService.GetCurrencyRateAsync(
                    withdrawalDto.Currency.ToString(), 
                    ((CurrencyType)account.Currency).ToString()); 

                withdrawalDto.Amount = withdrawalDto.Amount * currencyRate;  
            }

            decimal atmWithdrawalPercent = _configuration.GetValue<decimal>("TransactionFees:AtmWithdrawalPercent");

            decimal fee = withdrawalDto.Amount * (atmWithdrawalPercent / 100); 
            decimal totalAmountToDeduct = withdrawalDto.Amount + fee;

            if (account.Amount < totalAmountToDeduct)
            {
                return (false, "Not enough money.");
            }

            bool isBalanceUpdated = await _unitOfWork.CardRepository.UpdateAccountBalanceAsync(card.AccountId, totalAmountToDeduct);
            if (!isBalanceUpdated)
            {
                return (false, "Failed to update account balance.");
            }

            var transaction = new TransactionDetails
            {
                BankProfit = fee,
                Amount = withdrawalDto.Amount,
                FromAccountId = card.AccountId,
                ToAccountId = card.AccountId,
                CurrencyId = (int)withdrawalDto.Currency, // Set the selected currency
                IsATM = true
            };

            int insertedId = await _unitOfWork.TransactionDetailsRepository.CreateTransactionAsync(transaction);
            if (insertedId <= 0)
            {
                return (false, "Transaction could not be created, something happened!");
            }

            transaction.Id = insertedId;

            _unitOfWork.SaveChanges();
            return (true, "Withdrawal successful.");
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
        //tamar
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
    }
}
