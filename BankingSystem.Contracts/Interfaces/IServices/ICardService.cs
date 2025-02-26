using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.Contracts.Interfaces.IServices
{
    public interface ICardService
    {
        Task<(bool success, string message, Card? data)> CreateCardAsync(CreateCardDTO createCardDto);
        Task<(bool success, string message, List<CardWithIBANDTO> data)> SeeCardsAsync(string email);
        Task<(bool success, string message, SeeBalanceDTO data)> SeeBalanceAsync(string cardNumber, string pin);
        Task<(bool success, string message)> WithdrawAsync(WithdrawalDTO withdrawalDto);
        Task<(bool success, string message)> ChangeCardPINAsync(ChangeCardPINDTO changeCardDtp);
        Task<(bool success, string message)> CancelCardAsync(string cardNumber);
    }
}
