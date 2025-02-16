using BankingSystem.Contracts.Interfaces.IRepositories;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Infrastructure.DataAccess.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private IDbConnection _connection;
        private SqlTransaction _transaction;

        public void GiveCommandData(IDbConnection connection, SqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }
    }
}
