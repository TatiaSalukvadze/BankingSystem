﻿using BankingSystem.Contracts.Interfaces.IRepositories;
using System.Data;

namespace BankingSystem.Contracts.Interfaces
{
    public interface IUnitOfWork
    {
        IPersonRepository PersonRepository { get; set; }
        IAccountRepository AccountRepository { get; set; }
        ICardRepository CardRepository { get; set; }      
        ITransactionDetailsRepository TransactionDetailsRepository { get; set; }
        IDbTransaction Transaction();
        void BeginTransaction();
        void SaveChanges();
    }
}
