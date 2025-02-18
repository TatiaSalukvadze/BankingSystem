using BankingSystem.Contracts.DTOs;
using BankingSystem.Domain.Entities;

namespace BankingSystem.Contracts.Interfaces.IRepositories
{
    public interface ICardRepository 
    {
        Task<Card> GetCardAsync(string cardNumber);
        Task<List<CardWithIBANDTO>> GetCardsForPersonAsync(string email);
        Task<bool> CardNumberExists(string cardNumber);
        Task<int> CreateCardAsync(Card card);
        Task<bool> UpdateCardAsync(int cardId, string newPIN);
        Task<(decimal Amount, int Currency)> GetBalanceAsync(string cardNumber, string pin);
        Task<bool> UpdateAccountBalanceAsync(int accountId, decimal totalAmountToDeduct);
    }
}
