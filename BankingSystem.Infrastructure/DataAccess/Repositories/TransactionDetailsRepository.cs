using BankingSystem.Contracts.DTOs;
using BankingSystem.Contracts.Interfaces.IRepositories;
using BankingSystem.Domain.Entities;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using static System.Net.Mime.MediaTypeNames;

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
                var sql = "INSERT INTO TransactionDetails (BankProfit, Amount, FromAccountId, ToAccountId, CurrencyId, IsATM) " +
                    "OUTPUT INSERTED.Id " +
                    "VALUES (@BankProfit, @Amount, @FromAccountId, @ToAccountId, @CurrencyId, @IsATM)";
                insertedId = await _connection.ExecuteScalarAsync<int>(sql, transaction, _transaction);
            }

            return insertedId;
        }

        public async Task<decimal> GetTotalWithdrawnAmountIn24Hours(int accountId)
        {
            if (_connection != null && _transaction != null)
            {
                var sql = @"
                SELECT SUM(Amount) 
                FROM TransactionDetails 
                WHERE FromAccountId = @AccountId
                AND IsATM = 1
                AND PerformedAt >= DATEADD(HOUR, -24, GETDATE())";

                var result = await _connection.ExecuteScalarAsync<decimal>(sql, new { AccountId = accountId }, _transaction);
                return result;
            }
            return 0;
        }

        public async Task<TransactionCountDTO> NumberOfTransactionsAsync()
        {
            var result = new TransactionCountDTO();
            if (_connection != null && _transaction != null)
            {
                var sql = "SELECT (SELECT COUNT(*) FROM TransactionDetails WHERE PerformedAt > DATEADD(MONTH, -1, GETDATE())) AS LastMonthCount,"+
                    " (SELECT COUNT(*) FROM TransactionDetails WHERE PerformedAt > DATEADD(MONTH, -6, GETDATE())) AS LastSixMonthCount,"+
                    " (SELECT COUNT(*) FROM TransactionDetails WHERE PerformedAt > DATEADD(YEAR, -1, GETDATE())) AS LastYearCount";
                result = await _connection.QuerySingleAsync<TransactionCountDTO>(sql, transaction: _transaction);
                
            }

            return result;
        }

        public async Task<Dictionary<string, decimal>> AverageBankProfitAsyncAsync()
        {
            var result = new Dictionary<string, decimal>();
            if (_connection != null && _transaction != null)
            {
                var sql = "SELECT c.Type, ISNULL(AVG(BankProfit), 0) AS AverageProfit FROM TransactionDetails AS t " +
                    "RIGHT JOIN CurrencyType AS c ON c.Id = t.CurrencyId GROUP BY c.Type";
                var sqlResult = await _connection.QueryAsync<(string, decimal)>(sql, transaction: _transaction);
                result = sqlResult.ToDictionary(row => row.Item1, row => row.Item2);

            }

            return result;
        }
    }
}
