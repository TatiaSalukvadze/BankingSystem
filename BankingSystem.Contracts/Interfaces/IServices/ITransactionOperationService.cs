using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Contracts.Response;

namespace BankingSystem.Contracts.Interfaces.IServices
{
    public interface ITransactionOperationService
    {
        Task<SimpleResponse> OnlineTransactionAsync(CreateOnlineTransactionDTO createTransactionDto,
            string email, bool isSelfTransfer);
        Task<SimpleResponse> WithdrawAsync(WithdrawalDTO withdrawalDto);
    }
}
