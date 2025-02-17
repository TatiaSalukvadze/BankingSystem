using BankingSystem.Contracts.DTOs;

namespace BankingSystem.Contracts.Interfaces.IServices
{
    public interface IPersonService
    {
        Task<(bool Success, string Message, object? Data)> LoginPersonAsync(LoginDTO loginDto);
        Task<(bool Success, string Message, object? Data)> RegisterPersonAsync(RegisterPersonDTO registerDto);
        Task<(bool Success, string Message, Dictionary<string,int> statistics)> RegisteredPeopleStatisticsAsync();

    }
}
