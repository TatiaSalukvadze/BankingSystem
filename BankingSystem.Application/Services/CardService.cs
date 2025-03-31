﻿using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Contracts.Response;
using BankingSystem.Domain.Entities;
using System.Net.NetworkInformation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BankingSystem.Application.Services
{
    public class CardService : ICardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEncryptionService _encryptionService;
        private readonly IHashingService _hashingService;

        public CardService(IUnitOfWork unitOfWork, IEncryptionService encryptionService, IHashingService hashingService)
        {
            _unitOfWork = unitOfWork;
            _encryptionService = encryptionService;
            _hashingService = hashingService;
        }

        public async Task<Response<CreateCardDTO>> CreateCardAsync(CreateCardDTO createCardDto)
        {
            var response = new Response<CreateCardDTO>();
            var account = await _unitOfWork.AccountRepository.FindAccountByIBANAsync(createCardDto.IBAN);
            if (account == null)
            {
                return response.Set(false, "Account does not exist in the system!", null, 404);
            }

            bool cardNumberExists = await _unitOfWork.CardRepository.CardNumberExistsAsync(createCardDto.CardNumber);
            if (cardNumberExists)
            {
                return response.Set(false, "Card number already exists!", null, 409);
            }

            var card = new Card
            {
                AccountId = account.Id,
                CardNumber = _encryptionService.Encrypt(createCardDto.CardNumber),
                ExpirationDate = createCardDto.ExpirationDate,
                CVV = _encryptionService.Encrypt(createCardDto.CVV),
                PIN = _hashingService.HashValue(createCardDto.PIN)
            };

            int insertedId = await _unitOfWork.CardRepository.CreateCardAsync(card);
            if (insertedId <= 0)
            {
                return response.Set(false, "Card could not be created, something went wrong!", null, 400);
            }
            // card.Id = insertedId;

            return response.Set(true, "Card was created successfully!", createCardDto, 201);
        }

        public async Task<Response<List<CardWithIBANDTO>>> SeeCardsAsync(string email)
        {
            var response = new Response<List<CardWithIBANDTO>>();
            bool accountsExist = await _unitOfWork.AccountRepository.AccountExistForEmail(email);
            if (!accountsExist)
            {
                return response.Set(false, "You don't have accounts!", null, 400);
            }

            var cards = await _unitOfWork.CardRepository.GetCardsForPersonAsync(email);
            if (cards == null || cards.Count == 0)
            {
                return response.Set(false, "You don't have cards!", null, 404);
            }

            foreach (var card in cards) 
            {
                card.CardNumber = _encryptionService.Decrypt(card.CardNumber);
                card.CVV = _encryptionService.Decrypt(card.CVV);
            }

            return response.Set(true, "Cards For Account (IBAN) were found!", cards, 200);          
        }

        public async Task<Response<SeeBalanceDTO>> SeeBalanceAsync(CardAuthorizationDTO cardAuthorizationDto)
        {
            var response = new Response<SeeBalanceDTO>();
            var cardValidateResponse = await AuthorizeCardAsync(cardAuthorizationDto.CardNumber, cardAuthorizationDto.PIN);
            if (!cardValidateResponse.Success)
            {
                return response.Set(false, cardValidateResponse.Message, null, cardValidateResponse.StatusCode);
            }
            var hashedCardNumber = cardValidateResponse.Data.CardNumber;
            var encryptedPin = cardValidateResponse.Data.PIN;
            var balanceInfo = await _unitOfWork.CardRepository.GetBalanceAsync(hashedCardNumber, encryptedPin);
            if (balanceInfo is null || balanceInfo.Currency == 0)
            {
                return response.Set(false, "Unable to retrieve balance.", null, 400);
            }

            return response.Set(true, "Balance retrieved successfully.", balanceInfo, 200);
        }

        public async Task<SimpleResponse> ChangeCardPINAsync(ChangeCardPINDTO changeCardPINDto)
        {
            var response = new SimpleResponse();
            var cardValidateResponse = await AuthorizeCardAsync(changeCardPINDto.CardNumber, changeCardPINDto.PIN);
            if (!cardValidateResponse.Success)
            {
                return response.Set(false, cardValidateResponse.Message, cardValidateResponse.StatusCode);
            }

            var cardId = cardValidateResponse.Data.Id;
            var newPin = _hashingService.HashValue(changeCardPINDto.NewPIN);
            bool updated = await _unitOfWork.CardRepository.UpdateCardAsync(cardId, newPin);
            if (!updated)
            {
                return response.Set(false, "Card PIN could not be updated!", 400);
            }

            return response.Set(true, $"Card PIN was updated Successfully! New PIN: {changeCardPINDto.NewPIN}", 200);
        }

        public async Task<SimpleResponse> CancelCardAsync(string cardNumber)
        {
            var response = new SimpleResponse();
            var encryptedCardNumber = _encryptionService.Encrypt(cardNumber);
            var cardExists = await _unitOfWork.CardRepository.CardNumberExistsAsync(encryptedCardNumber);
            if (!cardExists)
            {
                return response.Set(false, "There is no Card for that Card Number!", 404);
            }

            var cardDeleted = await _unitOfWork.CardRepository.DeleteCardAsync(encryptedCardNumber);
            if (!cardDeleted)
            {
                return response.Set(false, "Card could not be canceled!", 400);
            }

            return response.Set(true, "Card was successfully canceled!", 200);
        }

        public async Task<Response<Card>> AuthorizeCardAsync(string CardNumber, string PIN)
        {
            var response = new Response<Card>();
            var encryptedCardNumber = _encryptionService.Encrypt(CardNumber);
            Card card = await _unitOfWork.CardRepository.GetCardAsync(encryptedCardNumber);

            if (card is null)
            {
                return response.Set(false, "Card was not found!", null, 404);
            }
            if (!_hashingService.VerifyValue(PIN, card.PIN))
            {
                return response.Set(false, "Incorrect PIN!", null, 400);
            }
            if (CheckCardExpired(card.ExpirationDate))
            {
                return response.Set(false, "Card is expired!", null, 400);
            }

            return response.Set(true, "Card validated", card, 200);
        }

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
