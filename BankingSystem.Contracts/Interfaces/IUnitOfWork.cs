using BankingSystem.Contracts.Interfaces.IRepositories;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Contracts.Interfaces
{
    public interface IUnitOfWork
    {
        IPersonRepository PersonRepository { get; set; }
        IAccountRepository AccountRepository { get; set; }

        ICardRepository CardRepository { get; set; }

        //SqlTransaction Transaction();
        //IDbConnection Connection();
        //IDbCommand CreateCommand();
        //void BeginTransaction();
        void SaveChanges();

    }
}
