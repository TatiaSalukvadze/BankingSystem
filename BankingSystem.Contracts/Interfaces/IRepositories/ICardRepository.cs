using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Domain.Entities;

namespace BankingSystem.Contracts.Interfaces.IRepositories
{
    public interface ICardRepository 
    {
        Task<Card> GetCardAsync(string cardNumber);
        Task<List<CardWithIBANDTO>> GetCardsForPersonAsync(string email);
        Task<bool> CardNumberExistsAsync(string cardNumber);
        Task<int> CreateCardAsync(Card card);
        Task<bool> UpdateCardAsync(int cardId, string newPIN);
        Task<SeeBalanceDTO> GetBalanceAsync(string cardNumber, string pin);
        Task<bool> UpdateAccountBalanceAsync(int accountId, decimal totalAmountToDeduct);
        Task<bool> DeleteCardAsync(string cardNumber);
    }
}
