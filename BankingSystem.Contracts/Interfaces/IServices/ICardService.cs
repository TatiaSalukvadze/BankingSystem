using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Contracts.Response;
using BankingSystem.Domain.Entities;

namespace BankingSystem.Contracts.Interfaces.IServices
{
    public interface ICardService
    {
        Task<Response<Card>> CreateCardAsync(CreateCardDTO createCardDto);
        Task<Response<List<CardWithIBANDTO>>> SeeCardsAsync(string email);
        Task<Response<SeeBalanceDTO>> SeeBalanceAsync(CardAuthorizationDTO cardAuthorizationDto);
        Task<SimpleResponse> ChangeCardPINAsync(ChangeCardPINDTO changeCardDtp);
        Task<SimpleResponse> CancelCardAsync(string cardNumber);
        Task<Response<Card>> AuthorizeCardAsync(string CardNumber, string PIN);
    }
}
