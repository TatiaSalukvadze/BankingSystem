﻿using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Domain.Entities;

namespace BankingSystem.Contracts.Interfaces.IServices
{
    public interface ITransactionOperationService
    {
        Task<(bool Success, string Message)> OnlineTransactionAsync(CreateOnlineTransactionDTO createTransactionDto,
            string email, bool isSelfTransfer);
        Task<(bool success, string message)> WithdrawAsync(WithdrawalDTO withdrawalDto);
    }
}
