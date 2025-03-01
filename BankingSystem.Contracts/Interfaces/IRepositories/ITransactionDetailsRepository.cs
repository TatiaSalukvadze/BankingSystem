using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.DTOs.Report;
using BankingSystem.Domain.Entities;

namespace BankingSystem.Contracts.Interfaces.IRepositories
{
    public interface ITransactionDetailsRepository 
    {
        Task<int> CreateTransactionAsync(TransactionDetails account);
        Task<decimal> GetTotalWithdrawnAmountIn24Hours(int accountId);
        Task<TransactionCountDTO> NumberOfTransactionsAsync();
        Task<List<TransactionCountChartDTO>> NumberOfTransactionsLastMonthAsync();
        Task<Dictionary<string, decimal>>  AverageBankProfitAsyncAsync();
        Task<List<BankProfitDTO>> GetBankProfitByTimePeriodAsync();
        Task<List<AtmWithdrawDTO>> GetTotalAtmWithdrawalsAsync();
        Task<Dictionary<string, decimal>> GetTotalIncomeAsync(DateTime fromDate, DateTime toDate, string email);
        Task<Dictionary<string, decimal>> GetTotalExpenseAsync(DateTime fromDate, DateTime toDate, string email);

    }
}
