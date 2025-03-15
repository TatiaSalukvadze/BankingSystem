using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Contracts.Response;

namespace BankingSystem.Contracts.Interfaces.IServices
{
    public interface IPersonService
    {
        Task<Response<object>> RegisterCustomPersonAsync(RegisterPersonDTO registerDto, string IdentityUserId);
        Task<Response<Dictionary<string,int>>> RegisteredPeopleStatisticsAsync();
    }
}
