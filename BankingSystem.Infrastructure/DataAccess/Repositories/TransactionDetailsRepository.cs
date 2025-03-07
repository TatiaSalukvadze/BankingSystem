using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.DTOs.Report;
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
                var sql = @"INSERT INTO TransactionDetails (BankProfit, Amount, FromAccountId, ToAccountId, Currency, IsATM) 
                    OUTPUT INSERTED.Id 
                    VALUES (@BankProfit, @Amount, @FromAccountId, @ToAccountId, @Currency, @IsATM)";
                insertedId = await _connection.ExecuteScalarAsync<int>(sql, transaction, _transaction);
            }

            return insertedId;
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
                    Currency,
                    SUM(CASE WHEN PerformedAt > DATEADD(MONTH, -1, GETDATE()) THEN BankProfit ELSE 0 END) AS LastMonthProfit,
                    SUM(CASE WHEN PerformedAt > DATEADD(MONTH, -6, GETDATE()) THEN BankProfit ELSE 0 END) AS LastSixMonthProfit,
                    SUM(CASE WHEN PerformedAt > DATEADD(YEAR, -1, GETDATE()) THEN BankProfit ELSE 0 END) AS LastYearProfit
                FROM TransactionDetails 
                GROUP BY Currency;";

                var result = await _connection.QueryAsync<BankProfitDTO>(sql, transaction: _transaction);
                return result.ToList();
            }

            return new List<BankProfitDTO>();
        }

        //tamr
        public async Task<List<TotalAtmWithdrawalDTO>> GetTotalAtmWithdrawalsAsync()
        {
            if (_connection != null && _transaction != null)
            {
                var sql = @"
                SELECT 
                    Currency,
                    SUM(Amount) AS TotalWithdrawnAmount
                FROM TransactionDetails
                WHERE IsATM = 1
                GROUP BY Currency;";

                var result = await _connection.QueryAsync<TotalAtmWithdrawalDTO>(sql, transaction: _transaction);
                return result.ToList();
            }

            return new List<TotalAtmWithdrawalDTO>();
        }

        //tatia
        public async Task<Dictionary<string, decimal>> AverageBankProfitAsyncAsync()
        {
            var result = new Dictionary<string, decimal>();
            if (_connection != null && _transaction != null)
            {
                var sql = @"SELECT Currency, ISNULL(AVG(BankProfit), 0) AS AverageProfit 
                            FROM TransactionDetails AS t 
                            GROUP BY Currency";
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
                //var sql = @"WITH LastMonthDays AS (
                //                SELECT CAST(DATEADD(MONTH, -1, GETDATE()) AS DATE) AS DayName

                //                UNION ALL

                //                SELECT DATEADD(DAY, 1, DayName) FROM LastMonthDays
                //                WHERE DayName < CAST(GETDATE() AS DATE)
                //            )

                //            SELECT lm.DayName,  COUNT(td.BankProfit) FROM TransactionDetails as td 
                //            right join LastMonthDays as lm on lm.DayName = CAST(PerformedAt AS DATE)
                //            Group by lm.DayName
                //            HAVING lm.DayName < CAST(GETDATE() AS DATE)";
                var sqlResult = await _connection.QueryAsync<(DateTime,int)>("SelectTransactionCountLastMonth", 
                    commandType: CommandType.StoredProcedure, transaction: _transaction);
                result = sqlResult.Select(row => new TransactionCountChartDTO { Date = DateOnly.FromDateTime(row.Item1), 
                    Count = row.Item2 })
                    .ToList();

            }

            return result;
        }

        public async Task<Dictionary<string, decimal>> GetTotalIncomeAsync(DateTime fromDate, DateTime toDate, string email)
        {
            var result = new Dictionary<string, decimal>();
            if (_connection != null && _transaction != null)
            {
                //var sql = @"SELECT ct.Type AS Currency, SUM(td.Amount) AS TotalIncome
                //    FROM TransactionDetails td
                //    JOIN Account a ON a.Id = td.ToAccountId  
                //    JOIN CurrencyType ct ON ct.Id = td.CurrencyId
                //    JOIN Person p ON p.Id = a.PersonId
                //    WHERE td.IsATM = 0 AND td.FromAccountId != td.ToAccountId 	
                //        AND td.PerformedAt >= @fromDate AND td.PerformedAt <=  @toDate
	               //     AND p.Email = @email
                //    GROUP BY ct.Type;";
                var sqlResult = await _connection.QueryAsync<(string, decimal)>("SelectTotalIncome",
                    new { email, fromDate, toDate }, commandType: CommandType.StoredProcedure, transaction: _transaction);
                result = sqlResult.ToDictionary(row => row.Item1, row => row.Item2);
            }
            if (result.Count <= 0)
            {
                result.Add("Total Income", 0);
            }
            return result;
        }

        public async Task<Dictionary<string, decimal>> GetTotalExpenseAsync(DateTime fromDate, DateTime toDate, string email)
        {
            var result = new Dictionary<string, decimal>();

            if (_connection != null && _transaction != null)
            {
                //var sql =   @"SELECT ct.[Type], SUM(td.BankProfit + td.Amount) AS Expense
                //              FROM TransactionDetails AS td 
                //              JOIN Account AS a ON td.FromAccountId = a.Id
                //              JOIN Person AS p ON p.Id = a.PersonId 
                //              JOIN CurrencyType AS ct ON ct.Id = td.CurrencyId
                //              WHERE p.Email = @email AND 
                //                td.FromAccountId != td.ToAccountId AND
                //                td.PerformedAt >= @fromDate AND td.PerformedAt <= @toDate
                //              GROUP BY ct.[Type]";
                var sqlResult = await _connection.QueryAsync<(string, decimal)>("SelectTotalExpense",
                    new { email, fromDate, toDate }, commandType: CommandType.StoredProcedure, transaction: _transaction);
                result = sqlResult.ToDictionary(row => row.Item1, row => row.Item2);

            }
            if (result.Count <= 0)
            {
                result.Add("TotalExpense", 0);
            }
            return result;
        }
    }
}
