﻿using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.DTOs.Report;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Domain.Entities;

namespace BankingSystem.Contracts.Interfaces.IServices
{
    public interface ITransactionService
    {
        Task<(bool Success, string Message, TransactionDetails Data)> OnlineTransactionAsync(CreateTransactionDTO createTransactionDto,
            string email, bool isSelfTransfer);
        Task<(bool Success, string Message, TransactionCountDTO Data)> NumberOfTransactionsAsync();
        Task<(bool Success, string Message, List<TransactionCountChartDTO> Data)> NumberOfTransactionsChartAsync();
        Task<(bool Success, string Message, Dictionary<string, decimal> Data)> AverageBankProfitAsync();
        Task<(bool Success, string Message, List<BankProfitDTO> Data)> GetBankProfitByTimePeriodAsync();
        Task<(bool Success, string Message, List<AtmWithdrawDTO> Data)> GetTotalAtmWithdrawalsAsync();
        Task<(bool Success, string Message, IncomeExpenseDTO Data)> TotalIncomeExpenseAsync(DateRangeDTO dateRangeDTO, string email);
    }
}
