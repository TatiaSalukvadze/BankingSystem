using BankingSystem.Contracts.DTOs;
using BankingSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.Contracts.Interfaces.IServices
{
    public interface ICardService
    {
        Task<(bool success, string message, object? data)> CreateCardAsync(CreateCardDTO createCardDto);
        Task<(bool success, string message, List<CardWithIBANDTO> data)> SeeCardsAsync(string email);
        bool CheckCardExpired(string expirationDate);
        Task<(bool success, string message)> ChangeCardPINAsync([FromForm] ChangeCardPINDTO changeCardDtp);
    }
}
