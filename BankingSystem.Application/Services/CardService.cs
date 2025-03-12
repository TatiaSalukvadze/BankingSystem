using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IExternalServices;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Domain.Entities;
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
                return (false, "You don't have accounts!", null);
            }

            var cards = await _unitOfWork.CardRepository.GetCardsForPersonAsync(email);
            if (cards == null || cards.Count == 0)
            {
                return (false, "You don't have cards!", null);
            }

            return (true, "Cards For Account (IBAN) were found!", cards);          
        }

        //tamar
        public async Task<(bool success, string message, SeeBalanceDTO data)> SeeBalanceAsync(CardAuthorizationDTO cardAuthorizationDto)
        {
            var (cardValidated, message, card) = await AuthorizeCardAsync(cardAuthorizationDto.CardNumber, cardAuthorizationDto.PIN);
            if (!cardValidated)
            {
                return (false, message, null);
            }

            var balanceInfo = await _unitOfWork.CardRepository.GetBalanceAsync(cardAuthorizationDto);

            if (balanceInfo is null || balanceInfo.Amount == 0 || balanceInfo.Currency == 0)
            {
                return (false, "Unable to retrieve balance.", null);
            }

            return (true, "Balance retrieved successfully.", balanceInfo);
        }

        //tatia
        public async Task<(bool success, string message)> ChangeCardPINAsync([FromForm] ChangeCardPINDTO changeCardDtp)
        {
            var (cardValidated, message, card) = await AuthorizeCardAsync(changeCardDtp.CardNumber, changeCardDtp.PIN);
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
        public async Task<(bool success, string message, Card card)> AuthorizeCardAsync(string CardNumber, string PIN)
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
