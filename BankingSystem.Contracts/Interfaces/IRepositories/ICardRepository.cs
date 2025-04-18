﻿using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Domain.Entities;

namespace BankingSystem.Contracts.Interfaces.IRepositories
{
    public interface ICardRepository : IRepository
    {
        Task<Card> GetCardAsync(string cardNumber);
        Task<List<CardWithIBANDTO>> GetCardsForPersonAsync(string email, int offset, int perPage);
        Task<int> GetCardsCountForPersonAsync(string email);
        Task<bool> CardNumberExistsAsync(string cardNumber);
        Task<int> CreateCardAsync(Card card);
        Task<bool> UpdateCardAsync(int cardId, string newPIN);
        Task<bool> DeleteCardAsync(string cardNumber);
        Task<SeeBalanceDTO> GetBalanceAsync(string cardNumber, string pin);
        Task<BalanceAndWithdrawalDTO> GetBalanceAndWithdrawnAmountAsync(string cardNumber, string pin);
    }
}
