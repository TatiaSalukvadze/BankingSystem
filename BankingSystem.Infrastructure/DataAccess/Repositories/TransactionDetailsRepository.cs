﻿using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.DTOs.Report;
using BankingSystem.Contracts.Interfaces.IRepositories;
using BankingSystem.Domain.Entities;
using Dapper;
using System.Data;

namespace BankingSystem.Infrastructure.DataAccess.Repositories
{
    public class TransactionDetailsRepository : ITransactionDetailsRepository
    {
        private readonly IDbConnection _connection;
        private IDbTransaction _transaction;

        public TransactionDetailsRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public void SetTransaction(IDbTransaction transaction)
        {
            _transaction = transaction;
        }

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

        public async Task<TransactionCountDTO> NumberOfTransactionsAsync()
        {
            var result = new TransactionCountDTO();
            if (_connection != null)
            {
                var sql = @"SELECT (SELECT COUNT(*) FROM TransactionDetails WHERE PerformedAt > DATEADD(MONTH, -1, GETDATE())) AS LastMonthCount,
                     (SELECT COUNT(*) FROM TransactionDetails WHERE PerformedAt > DATEADD(MONTH, -6, GETDATE())) AS LastSixMonthCount,
                     (SELECT COUNT(*) FROM TransactionDetails WHERE PerformedAt > DATEADD(YEAR, -1, GETDATE())) AS LastYearCount";
                result = await _connection.QuerySingleAsync<TransactionCountDTO>(sql);
            }
            return result;
        }

        public async Task<List<BankProfitDTO>> GetBankProfitByTimePeriodAsync()
        {
            if (_connection != null)
            {
                var sql = @"SELECT * FROM BankProfitView";
                var result = await _connection.QueryAsync<BankProfitDTO>(sql);
                return result.ToList();
            }
            return [];
        }

        public async Task<List<TotalAtmWithdrawalDTO>> GetTotalAtmWithdrawalsAsync()
        {
            if (_connection != null)
            {
                var sql = @"
                SELECT 
                    Currency,
                    SUM(Amount) AS TotalWithdrawnAmount
                FROM TransactionDetails
                WHERE IsATM = 1
                GROUP BY Currency;";
                var result = await _connection.QueryAsync<TotalAtmWithdrawalDTO>(sql);
                return result.ToList();
            }
            return [];
        }

        public async Task<Dictionary<string, decimal>> AverageBankProfitAsync()
        {
            var result = new Dictionary<string, decimal>();
            if (_connection != null)
            {
                var sql = @"SELECT Currency, ISNULL(AVG(BankProfit), 0) AS AverageProfit 
                            FROM TransactionDetails AS t 
                            GROUP BY Currency";
                var sqlResult = await _connection.QueryAsync<(string, decimal)>(sql);
                result = sqlResult.ToDictionary(row => row.Item1, row => row.Item2);
            }
            return result;
        }

        public async Task<List<TransactionCountChartDTO>> NumberOfTransactionsLastMonthAsync()
        {
            var result = new List<TransactionCountChartDTO>();
            if (_connection != null)
            {
                var sql = @"SELECT(CAST (PerformedAt AS DATE)) as [Date],  COUNT(*) AS TransactionCount 
                            FROM TransactionDetails
                            WHERE PerformedAt > CAST(DATEADD(MONTH, -1, GETDATE()) AS DATE) 
                                AND PerformedAt < GETDATE()
                            GROUP BY (CAST (PerformedAt AS DATE)) 
                            ORDER BY [Date]";
                var sqlResult = await _connection.QueryAsync<(DateTime,int)>(sql);
                result = sqlResult.Select(row => new TransactionCountChartDTO { Date = DateOnly.FromDateTime(row.Item1), 
                    Count = row.Item2 }).ToList();
            }
            return result;
        }

        public async Task<Dictionary<string, decimal>> GetTotalIncomeAsync(DateTime fromDate, DateTime toDate, string email)
        {
            var result = new Dictionary<string, decimal>();
            if (_connection != null)
            {
                var sqlResult = await _connection.QueryAsync<(string, decimal)>("SelectTotalIncome",
                    new { email, fromDate, toDate }, commandType: CommandType.StoredProcedure);
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
            if (_connection != null)
            {
                var sqlResult = await _connection.QueryAsync<(string, decimal)>("SelectTotalExpense",
                    new { email, fromDate, toDate }, commandType: CommandType.StoredProcedure);
                result = sqlResult.ToDictionary(row => row.Item1, row => row.Item2);
            }
            if (result.Count <= 0)
            {
                result.Add("Total Expense", 0);
            }
            return result;
        }
    }
}
