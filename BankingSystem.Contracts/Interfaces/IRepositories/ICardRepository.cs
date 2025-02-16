using BankingSystem.Domain.Entities;

namespace BankingSystem.Contracts.Interfaces.IRepositories
{
    public interface ICardRepository 
    {
        //Task<bool> AccountExists(int accountId);
        Task<bool> CardNumberExists(string cardNumber);
        Task<int> CreateCardAsync(Card card);
    }
}
