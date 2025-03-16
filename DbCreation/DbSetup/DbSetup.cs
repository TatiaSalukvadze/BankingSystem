using BankingSystem.Infrastructure.Identity;
using Dapper;
using DbCreation.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace DbCreation.DbSetup
{
    internal class DbSetup : IDbSetup
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IDbConnection _serverConnection;
        private readonly IDbConnection _dbConnection;
        private readonly ILogger _logger;
        private string _basePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName, "Queries");
        
        public DbSetup(ApplicationDbContext dbContext, IDbConnectionFactory dbConnectionFactory, ILogger<DbSetup> logger)
        {
            _dbContext = dbContext;
            _dbConnection = dbConnectionFactory.CreateDbConnection();
            _serverConnection = dbConnectionFactory.CreateServerConnection();
            _logger = logger;
        }

        public async Task CreateDbAndTablesAsync()
        {
            bool dbExists = false;
            using (_serverConnection)
            {
                _serverConnection.Open();
                string dbName = "Test";
                var checkDbSql = "SELECT CASE WHEN EXISTS (SELECT name FROM master.sys.databases WHERE name = @dbName) THEN 1 ELSE 0 END AS DbExists";
                dbExists = await _serverConnection.ExecuteScalarAsync<bool>(checkDbSql, new { dbName });
            }
            await _dbContext.Database.MigrateAsync();

            if (dbExists)
            {
                _logger.LogInformation("Db already exists!");
                return;
            }
  
            using (_dbConnection)
            {
                _dbConnection.Open();
                using var transaction = _dbConnection.BeginTransaction();
                try
                {
                    await CreateCustomTablesAsync(transaction);
                    await CreateProceduresAsync(transaction);
                    await CreateViewsAsync(transaction);
                    transaction.Commit();
                    _logger.LogInformation("Database setup completed successfully.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError("exception happened during db setupt: {ExceptionMessage},{StackTrace}", ex.Message, ex.StackTrace);
                }
            }
        
        }

        private async Task CreateCustomTablesAsync(IDbTransaction transaction)
        {
            var createPersonTable = await File.ReadAllTextAsync(Path.Combine(_basePath, "PersonTable.sql"));
            var createAccountTable = await File.ReadAllTextAsync(Path.Combine(_basePath, "AccountTable.sql"));
            var createCardTable = await File.ReadAllTextAsync(Path.Combine(_basePath, "CardTable.sql"));
            var createTransactionDetailsTable = await File.ReadAllTextAsync(Path.Combine(_basePath, "TransactionDetailsTable.sql"));

            await _dbConnection.ExecuteAsync(createPersonTable, transaction: transaction);
            await _dbConnection.ExecuteAsync(createAccountTable, transaction: transaction);
            await _dbConnection.ExecuteAsync(createCardTable, transaction: transaction);
            await _dbConnection.ExecuteAsync(createTransactionDetailsTable, transaction: transaction);
            _logger.LogInformation("Custom Tables created!");
        }

        private async Task CreateProceduresAsync(IDbTransaction transaction)
        {
            var selectTotalExpenseProcedure = await File.ReadAllTextAsync(Path.Combine(_basePath, "SelectTotalExpenseProcedure.sql"));
            var selectTotalIncomeProcedure = await File.ReadAllTextAsync(Path.Combine(_basePath, "SelectTotalIncomeProcedure.sql"));
            var selectTransactionCountProcedure = await File.ReadAllTextAsync(Path.Combine(_basePath, "SelectTransactionCountProcedure.sql"));

            await _dbConnection.ExecuteAsync(selectTotalExpenseProcedure, transaction: transaction);
            await _dbConnection.ExecuteAsync(selectTotalIncomeProcedure, transaction: transaction);
            await _dbConnection.ExecuteAsync(selectTransactionCountProcedure, transaction: transaction);
            _logger.LogInformation("Procedures created!");
        }
        private async Task CreateViewsAsync(IDbTransaction transaction)
        {
            var bankProfitView = await File.ReadAllTextAsync(Path.Combine(_basePath, "BankProfitView.sql"));

            await _dbConnection.ExecuteAsync(bankProfitView, transaction: transaction);
            _logger.LogInformation("Views created!");
        }
    }
}
