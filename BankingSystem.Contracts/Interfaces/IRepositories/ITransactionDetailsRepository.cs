using BankingSystem.Contracts.DTOs;
using BankingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Contracts.Interfaces.IRepositories
{
    public interface ITransactionDetailsRepository 
    {
        Task<int> CreateTransactionAsync(TransactionDetails account);
        Task<TransactionCountDTO> NumberOfTransactionsAsync();
    }
}
