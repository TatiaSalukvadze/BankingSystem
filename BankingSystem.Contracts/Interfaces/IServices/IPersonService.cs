using BankingSystem.Contracts.DTOs.OnlineBank;

namespace BankingSystem.Contracts.Interfaces.IServices
{
    public interface IPersonService
    {
        Task<(bool Success, string Message, object? Data)> RegisterCustomPersonAsync(RegisterPersonDTO registerDto, string IdentityUserId);
        Task<(bool Success, string Message, Dictionary<string,int> statistics)> RegisteredPeopleStatisticsAsync();

    }
}
