using BankingSystem.Contracts.DTOs;

namespace BankingSystem.Contracts.Interfaces.IServices
{
    public interface ICardService
    {
        Task<(bool success, string? message, object data)> CreateCardAsync(CreateCardDTO createCardDto);
    }
}
