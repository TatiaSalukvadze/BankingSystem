using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.DTOs.Report;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Contracts.Response;

namespace BankingSystem.Contracts.Interfaces.IServices
{
    public interface ITransactionDetailsService
    {
        Task<SimpleResponse> CreateTransactionAsync(decimal bankProfit,
                decimal amount, int fromAccountId, int toAccountId, string currencyId, bool IsATM = false);      
        Task<Response<TransactionCountDTO>> NumberOfTransactionsAsync();
        Task<Response<List<TransactionCountChartDTO>>> NumberOfTransactionsChartAsync();
        Task<Response<Dictionary<string, decimal>>> AverageBankProfitAsync();
        Task<Response<List<BankProfitDTO>>> BankProfitByTimePeriodAsync();
        Task<Response<List<TotalAtmWithdrawalDTO>>> TotalAtmWithdrawalsAsync();
        Task<Response<IncomeExpenseDTO>> TotalIncomeExpenseAsync(DateRangeDTO dateRangeDTO, string email);
        Task<Response<TransferAmountCalculationDTO>> CalculateTransferAmountAsync(
            string fromCurrency, string toCurrency, decimal amountToTransfer, bool isSelfTransfer);
        Task<Response<AtmWithdrawalCalculationDTO>> CalculateATMWithdrawalTransactionAsync(
            string cardNumber, string pin, decimal withdrawalAmount, string withdrawalCurrency);
    }
}
