using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Contracts.Response;
using BankingSystem.Domain.Entities;

namespace BankingSystem.Contracts.Interfaces.IServices
{
    public interface ICardService
    {
        Task<Response<CreateCardDTO>> CreateCardAsync(CreateCardDTO createCardDto);
        Task<Response<PagingResponseDTO<CardWithIBANDTO>>> SeeCardsAsync(string email, int page, int perPage);
        Task<Response<SeeBalanceDTO>> SeeBalanceAsync(CardAuthorizationDTO cardAuthorizationDto);
        Task<SimpleResponse> ChangeCardPINAsync(ChangeCardPINDTO changeCardPINDto);
        Task<SimpleResponse> CancelCardAsync(DeleteCardDTO deleteCardDto);
        Task<Response<Card>> AuthorizeCardAsync(string CardNumber, string PIN);
    }
}
