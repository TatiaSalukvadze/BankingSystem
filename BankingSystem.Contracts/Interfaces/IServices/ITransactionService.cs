using BankingSystem.Contracts.DTOs;

namespace BankingSystem.Contracts.Interfaces.IServices
{
    public interface ITransactionService
    {
        Task<(bool Success, string Message, object Data)> OnlineTransactionAsync(CreateTransactionDTO createTransactionDto,
            string email, bool isSelfTransfer);
        Task<(bool Success, string Message, object Data)> NumberOfTransactionsAsync();
        Task<(bool Success, string Message, object Data)> NumberOfTransactionsChartAsync();
        Task<(bool Success, string Message, object Data)> AverageBankProfitAsync();
        Task<(bool Success, string Message, object Data)> GetBankProfitByTimePeriodAsync();
        Task<(bool Success, string Message, object Data)> GetTotalAtmWithdrawalsAsync();
    }
}
