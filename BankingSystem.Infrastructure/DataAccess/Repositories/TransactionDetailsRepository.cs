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
    public class TransactionDetailsRepository : ITransactionDetailsRepository
    {
        private SqlConnection _connection;
        private IDbTransaction _transaction;

        public TransactionDetailsRepository(SqlConnection connection, IDbTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }
    }
}
