﻿using BankingSystem.Contracts.DTOs;

namespace BankingSystem.Contracts.Interfaces.IServices
{
    public interface ITransactionService
    {
        Task<(bool Success, string Message, object Data)> OnlineTransactionAsync(CreateTransactionDTO createTransactionDto,
            string email, bool isSelfTransfer);
        Task<(bool Success, string Message, object Data)> NumberOfTransactionsAsync();
        Task<(bool Success, string Message, object Data)> AverageBankProfitAsync();
    }
}
