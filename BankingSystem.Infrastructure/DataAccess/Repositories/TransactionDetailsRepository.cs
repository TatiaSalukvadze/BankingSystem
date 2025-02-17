using BankingSystem.Contracts.Interfaces.IRepositories;
using BankingSystem.Domain.Entities;
using Dapper;
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

        public async Task<int> CreateTransactionAsync(TransactionDetails transaction)
        {
            int insertedId = 0;
            if (_connection != null && _transaction != null)
            {
                var sql = "INSERT INTO TransactionDetails (BankProfit, Amount, FromAccountId, ToAccountId, CurrencyId) " +
                    "OUTPUT INSERTED.Id " +
                    "VALUES (@BankProfit, @Amount, @FromAccountId, @ToAccountId, @CurrencyId)";
                insertedId = await _connection.ExecuteScalarAsync<int>(sql, transaction, _transaction);
            }

            return insertedId;
        }
    }
}
