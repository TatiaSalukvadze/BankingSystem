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

        //tatia
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
        //tamar
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

        //tatia
        public async Task<TransactionCountDTO> NumberOfTransactionsAsync()
        {
            var result = new TransactionCountDTO();
            if (_connection != null && _transaction != null)
            {
                var sql = @"SELECT (SELECT COUNT(*) FROM TransactionDetails WHERE PerformedAt > DATEADD(MONTH, -1, GETDATE())) AS LastMonthCount,
                     (SELECT COUNT(*) FROM TransactionDetails WHERE PerformedAt > DATEADD(MONTH, -6, GETDATE())) AS LastSixMonthCount,
                     (SELECT COUNT(*) FROM TransactionDetails WHERE PerformedAt > DATEADD(YEAR, -1, GETDATE())) AS LastYearCount";
                result = await _connection.QuerySingleAsync<TransactionCountDTO>(sql, transaction: _transaction);
                
            }

            return result;
        }

        //tamr
        public async Task<List<BankProfitDTO>> GetBankProfitByTimePeriodAsync()
        {
            if (_connection != null && _transaction != null)
            {
                var sql = @"
                SELECT 
                    C.Type AS Currency,
                    SUM(CASE WHEN TD.PerformedAt > DATEADD(MONTH, -1, GETDATE()) THEN TD.BankProfit ELSE 0 END) AS LastMonthProfit,
                    SUM(CASE WHEN TD.PerformedAt > DATEADD(MONTH, -6, GETDATE()) THEN TD.BankProfit ELSE 0 END) AS LastSixMonthProfit,
                    SUM(CASE WHEN TD.PerformedAt > DATEADD(YEAR, -1, GETDATE()) THEN TD.BankProfit ELSE 0 END) AS LastYearProfit
                FROM TransactionDetails TD
                JOIN CurrencyType C ON TD.CurrencyId = C.Id
                GROUP BY C.Type;";

                var result = await _connection.QueryAsync<BankProfitDTO>(sql, transaction: _transaction);
                return result.ToList();
            }

            return new List<BankProfitDTO>();
        }

        //tamr
        public async Task<List<AtmWithdrawDTO>> GetTotalAtmWithdrawalsAsync()
        {
            if (_connection != null && _transaction != null)
            {
                var sql = @"
                SELECT 
                    C.Type AS Currency,
                    SUM(TD.Amount) AS TotalWithdrawnAmount
                FROM TransactionDetails AS TD
                JOIN CurrencyType C ON TD.CurrencyId = C.Id
                WHERE IsATM = 1
                GROUP BY C.Type;";

                var result = await _connection.QueryAsync<AtmWithdrawDTO>(sql, transaction: _transaction);
                return result.ToList();
            }

            return new List<AtmWithdrawDTO>();
        }

        //tatia
        public async Task<Dictionary<string, decimal>> AverageBankProfitAsyncAsync()
        {
            var result = new Dictionary<string, decimal>();
            if (_connection != null && _transaction != null)
            {
                var sql = @"SELECT c.Type, ISNULL(AVG(BankProfit), 0) AS AverageProfit FROM TransactionDetails AS t 
                    RIGHT JOIN CurrencyType AS c ON c.Id = t.CurrencyId GROUP BY c.Type";
                var sqlResult = await _connection.QueryAsync<(string, decimal)>(sql, transaction: _transaction);
                result = sqlResult.ToDictionary(row => row.Item1, row => row.Item2);

            }

            return result;
        }

        //tatia
        public async Task<List<TransactionCountChartDTO>> NumberOfTransactionsLastMonthAsync()
        {
            var result = new List<TransactionCountChartDTO>();
            if (_connection != null && _transaction != null)
            {
                var sql = @"WITH LastMonthDays AS (
                                SELECT CAST(DATEADD(MONTH, -1, GETDATE()) AS DATE) AS DayName

                                UNION ALL

                                SELECT DATEADD(DAY, 1, DayName) FROM LastMonthDays
                                WHERE DayName < CAST(GETDATE() AS DATE)
                            )

                            SELECT lm.DayName,  COUNT(td.BankProfit) FROM TransactionDetails as td 
                            right join LastMonthDays as lm on lm.DayName = CAST(PerformedAt AS DATE)
                            Group by lm.DayName
                            HAVING lm.DayName < CAST(GETDATE() AS DATE)";
                var sqlResult = await _connection.QueryAsync<(DateTime,int)>(sql, transaction: _transaction);
                result = sqlResult.Select(row => new TransactionCountChartDTO { Date = DateOnly.FromDateTime(row.Item1), 
                    Count = row.Item2 })
                    .ToList();

            }

            return result;
        }
    }
}
