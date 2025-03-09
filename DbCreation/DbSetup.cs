using BankingSystem.Infrastructure.Identity;
using Dapper;
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

namespace DbCreation
{
    internal class DbSetup
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IDbConnection _connection;
        private string _basePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName, "Queries");
        public DbSetup(ApplicationDbContext dbContext, IDbConnection dbConnection) {
            _connection = dbConnection;
            _dbContext = dbContext;
            //if (_connection.State == ConnectionState.Closed)
            //{
            //    _connection.Open();
            //}

            _connection.Open();
        }

        public async Task CreateDbAndTables()
        {

            //var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            if(_connection is not null && _connection.State == ConnectionState.Open)
            {
                string dbName = "Test";
                var checkDbSql = "SELECT CASE WHEN EXISTS (SELECT name FROM master.sys.databases WHERE name = @dbName) THEN 1 ELSE 0 END AS DbExists";
                bool exists = await _connection.ExecuteScalarAsync<bool>(checkDbSql, new { dbName });

                await _dbContext.Database.MigrateAsync();

                if (exists)
                {
                    return;
                }
           
                string switchToDb = $"USE @dbName";
                await _connection.ExecuteAsync(switchToDb, new { dbName });

                var createPersonTable = await File.ReadAllTextAsync(Path.Combine(_basePath, "PersonTable.sql"));
                var res = await _connection.ExecuteAsync(createPersonTable);

                _connection.Close();
            }
            

        }

    }
}
