using BankingSystem.Contracts.DTOs;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.Application.Services
{
    public class CardService : ICardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(bool success, string message)> ChangeCardPINAsync([FromForm] ChangeCardPINDTO changeCardDtp)
        {
           Card card = await _unitOfWork.CardRepository.GetCardAsync(changeCardDtp.CardNumber);

            if (card is null)
            {
                return (false, "Card was not found!");
            }
            if (CheckCardExpired(card.ExpirationDate))
            {
                return (false, "Card is expired!");
            }

            bool updated = await _unitOfWork.CardRepository.UpdateCardAsync(card.Id, changeCardDtp.NewPIN);
            if (!updated)
            {
                return (false, "Card PIN could be updated!");
            }
            _unitOfWork.SaveChanges();
            return (true, $"Card PIN was updated Successfully! New PIN: {changeCardDtp.NewPIN}");
        }
        public bool CheckCardExpired(string expirationDate)
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
        public async Task<(bool success, string message, object? data)> CreateCardAsync(CreateCardDTO createCardDto)
        {
            try
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
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool success, string message, List<CardWithIBANDTO> data)> SeeCardsAsync(string email)
        {
            try
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
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}
