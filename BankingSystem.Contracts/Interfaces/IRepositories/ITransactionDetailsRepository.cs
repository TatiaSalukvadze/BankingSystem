﻿using BankingSystem.Contracts.DTOs;
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
        Task<Dictionary<string, decimal>> GetTotalIncomeAsync(DateTime fromTime, DateTime toTime, string email);
        Task<Dictionary<string, decimal>> GetTotalExpenseAsync(DateTime fromTime, DateTime toTime, string email);
    }
}
