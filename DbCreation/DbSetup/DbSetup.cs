using BankingSystem.Infrastructure.Identity;
using Dapper;
using DbCreation.Helpers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DbCreation.DbSetup
{
    internal class DbSetup : IDbSetup
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IDbConnection _serverConnection;
        private readonly IDbConnection _dbConnection;
        private string _basePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName, "Queries");
        public DbSetup(ApplicationDbContext dbContext, IDbConnectionFactory dbConnectionFactory)
        {
            _dbContext = dbContext;
            _dbConnection = dbConnectionFactory.CreateDbConnection();
            _serverConnection = dbConnectionFactory.CreateServerConnection();

        }

        public async Task CreateDbAndTables()
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
                return;
            }
            using (_dbConnection)
            {
                _dbConnection.Open();
                await CreateCustomTables();
                await CreateProcedures();
            }

        }
        private async Task CreateCustomTables()
        {
            var createPersonTable = await File.ReadAllTextAsync(Path.Combine(_basePath, "PersonTable.sql"));
            var createAccountTable = await File.ReadAllTextAsync(Path.Combine(_basePath, "AccountTable.sql"));
            var createCardTable = await File.ReadAllTextAsync(Path.Combine(_basePath, "CardTable.sql"));
            var createTransactionDetailsTable = await File.ReadAllTextAsync(Path.Combine(_basePath, "TransactionDetailsTable.sql"));


            var res = await _dbConnection.ExecuteAsync(createPersonTable);
            await _dbConnection.ExecuteAsync(createAccountTable);
            await _dbConnection.ExecuteAsync(createCardTable);
            await _dbConnection.ExecuteAsync(createTransactionDetailsTable);

        }
        private async Task CreateProcedures()
        {
            var SelectTotalExpenseProcedure = await File.ReadAllTextAsync(Path.Combine(_basePath, "SelectTotalExpenseProcedure.sql"));
            var SelectTotalIncomeProcedure = await File.ReadAllTextAsync(Path.Combine(_basePath, "SelectTotalIncomeProcedure.sql"));
            var SelectTransactionCountProcedure = await File.ReadAllTextAsync(Path.Combine(_basePath, "SelectTransactionCountProcedure.sql"));

            await _dbConnection.ExecuteAsync(SelectTotalExpenseProcedure);
            await _dbConnection.ExecuteAsync(SelectTotalIncomeProcedure);
            await _dbConnection.ExecuteAsync(SelectTransactionCountProcedure);
        }
    }
}
