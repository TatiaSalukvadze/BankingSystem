using BankingSystem.Contracts.DTOs;
using BankingSystem.Domain.Entities;

namespace BankingSystem.Contracts.Interfaces.IRepositories
{
    public interface ITransactionDetailsRepository 
    {
        Task<int> CreateTransactionAsync(TransactionDetails account);
        Task<decimal> GetTotalWithdrawnAmountIn24Hours(int accountId);
        Task<TransactionCountDTO> NumberOfTransactionsAsync();
        Task<Dictionary<DateOnly, int>> NumberOfTransactionsLastMonthAsync();
        Task<Dictionary<string, decimal>>  AverageBankProfitAsyncAsync();
    }
}
