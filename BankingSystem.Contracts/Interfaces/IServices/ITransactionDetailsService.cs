using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.DTOs.Report;
using BankingSystem.Contracts.DTOs.UserBanking;

namespace BankingSystem.Contracts.Interfaces.IServices
{
    public interface ITransactionDetailsService
    {
        Task<(bool Success, string Message)> CreateTransactionAsync(decimal bankProfit,
                decimal amount, int fromAccountId, int toAccountId, string currencyId, bool IsATM = false);      
        Task<(bool Success, string Message, TransactionCountDTO Data)> NumberOfTransactionsAsync();
        Task<(bool Success, string Message, List<TransactionCountChartDTO> Data)> NumberOfTransactionsChartAsync();
        Task<(bool Success, string Message, Dictionary<string, decimal> Data)> AverageBankProfitAsync();
        Task<(bool Success, string Message, List<BankProfitDTO> Data)> GetBankProfitByTimePeriodAsync();
        Task<(bool Success, string Message, List<TotalAtmWithdrawalDTO> Data)> GetTotalAtmWithdrawalsAsync();
        Task<(bool Success, string Message, IncomeExpenseDTO Data)> TotalIncomeExpenseAsync(DateRangeDTO dateRangeDTO, string email);
        Task<(bool Success, string Message, TransferAmountCalculationDTO Data)> CalculateTransferAmountAsync(
            string fromCurrency, string toCurrency, decimal amountToTransfer, bool isSelfTransfer);
        Task<(bool success, string message, AtmWithdrawalCalculationDTO Data)> CalculateATMWithdrawalTransactionAsync(
            string cardNumber, string pin, decimal withdrawalAmount, string withdrawalCurrency);
    }
}
