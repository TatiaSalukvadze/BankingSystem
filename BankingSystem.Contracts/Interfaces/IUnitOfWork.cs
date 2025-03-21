using BankingSystem.Contracts.Interfaces.IRepositories;
using System.Data;

namespace BankingSystem.Contracts.Interfaces
{
    public interface IUnitOfWork
    {
        IPersonRepository PersonRepository { get;  }
        IAccountRepository AccountRepository { get;  }
        ICardRepository CardRepository { get;  }      
        ITransactionDetailsRepository TransactionDetailsRepository { get; }
        IDbTransaction Transaction();
        void BeginTransaction();
        void SaveChanges();
    }
}
