using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Contracts.Response;
using BankingSystem.Domain.Entities;

namespace BankingSystem.Application.Services
{
    public class CardService : ICardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<Card>> CreateCardAsync(CreateCardDTO createCardDto)
        {
            var response = new Response<Card>();
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
                CardNumber = createCardDto.CardNumber,
                ExpirationDate = createCardDto.ExpirationDate,
                CVV = createCardDto.CVV,
                PIN = createCardDto.PIN
            };

            int insertedId = await _unitOfWork.CardRepository.CreateCardAsync(card);
            if (insertedId <= 0)
            {
                return response.Set(false, "Card could not be created, something went wrong!", null, 400);
            }
            card.Id = insertedId;
 
            return response.Set(true, "Card was created successfully!", card, 201);
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

            var balanceInfo = await _unitOfWork.CardRepository.GetBalanceAsync(cardAuthorizationDto);
            if (balanceInfo is null || balanceInfo.Amount == 0 || balanceInfo.Currency == 0)
            {
                return response.Set(false, "Unable to retrieve balance.", null, 400);
            }

            return response.Set(true, "Balance retrieved successfully.", balanceInfo, 200);
        }

        public async Task<SimpleResponse> ChangeCardPINAsync(ChangeCardPINDTO changeCardDtp)
        {
            var response = new SimpleResponse();
            var cardValidateResponse = await AuthorizeCardAsync(changeCardDtp.CardNumber, changeCardDtp.PIN);
            if (!cardValidateResponse.Success)
            {
                return response.Set(false, cardValidateResponse.Message, cardValidateResponse.StatusCode);
            }

            var card = cardValidateResponse.Data;
            bool updated = await _unitOfWork.CardRepository.UpdateCardAsync(card.Id, changeCardDtp.NewPIN);
            if (!updated)
            {
                return response.Set(false, "Card PIN could not be updated!", 400);
            }

            return response.Set(true, $"Card PIN was updated Successfully! New PIN: {changeCardDtp.NewPIN}", 200);
        }

        public async Task<SimpleResponse> DeleteCardAsync(string cardNumber)
        {
            var response = new SimpleResponse();
            var cardExists = await _unitOfWork.CardRepository.CardNumberExistsAsync(cardNumber);
            if (!cardExists)
            {
                return response.Set(false, "There is no Card for that Card Number!", 404);
            }

            var cardDeleted = await _unitOfWork.CardRepository.DeleteCardAsync(cardNumber);
            if (!cardDeleted)
            {
                return response.Set(false, "Card could not be canceled!", 400);
            }

            return response.Set(true, "Card was successfully canceled!", 200);
        }

        public async Task<Response<Card>> AuthorizeCardAsync(string CardNumber, string PIN)
        {
            var response = new Response<Card>();
            Card card = await _unitOfWork.CardRepository.GetCardAsync(CardNumber);

            if (card is null)
            {
                return response.Set(false, "Card was not found!", null, 404);
            }
            if (card.PIN != PIN)
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
