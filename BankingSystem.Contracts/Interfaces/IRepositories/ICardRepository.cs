using BankingSystem.Contracts.DTOs;
using BankingSystem.Domain.Entities;

namespace BankingSystem.Contracts.Interfaces.IRepositories
{
    public interface ICardRepository 
    {

        Task<List<CardWithIBANDTO>> SeeCardsAsync(string email);
        Task<bool> CardNumberExists(string cardNumber);
        Task<int> CreateCardAsync(Card card);
    }
}
