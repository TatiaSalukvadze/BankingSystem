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
    public class AccountRepository : IAccountRepository
    {
        private SqlConnection _connection;
        private IDbTransaction _transaction;

        public AccountRepository(SqlConnection connection, IDbTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }


        public async Task<int> CreateAccountAsync(Account account)
        {
            int insertedId = 0;
            if (_connection != null && _transaction != null)
            {
                var sql = "INSERT INTO Account (PersonId, IBAN, Amount, CurrencyId) " +
                    "OUTPUT INSERTED.Id " +
                    "VALUES (@PersonId, @IBAN, @Amount, @CurrencyId)";
                insertedId = await _connection.ExecuteScalarAsync<int>(sql, account, _transaction);
            }

            return insertedId;
        }

        public async Task<bool> IBANExists(string IBAN)
        {
            bool exists = false;
            if (_connection != null && _transaction != null)
            {
                var sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM Account WHERE IBAN = @IBAN) THEN 1 ELSE 0 END AS IBANExists";
                exists = await _connection.ExecuteScalarAsync<bool>(sql, new {IBAN}, _transaction);
            }

            return exists;

        }

        public async Task<bool> AccountExistForEmail(string email)
        {
            bool exists = false;
            if (_connection != null && _transaction != null)
            {
                var sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM Account as a WHERE @email = " +
                    "(SELECT TOP 1 Email FROM Person WHERE Id = a.PersonId)) THEN 1 ELSE 0 END AS AccountExists";
                exists = await _connection.ExecuteScalarAsync<bool>(sql, new { email }, _transaction);
            }

            return exists;
        }

        public async Task<List<(int, string)>> SeeAccountsByEmail(string email)
        {
            var result = new List<(int, string)>{};
            if (_connection != null && _transaction != null)
            {
                var sql = "SELECT Id, IBAN FROM Account AS a WHERE " +
                    "@email = (SELECT TOP 1 Email FROM Person WHERE Id = a.PersonId);";
                result = (await _connection.QueryAsync<(int,string)>(sql, new { email }, _transaction)).ToList();
            }

            return result;
        }

        public async Task<string> GetCurrencyNameById(int currencyId)
        {
            if (_connection != null && _transaction != null)
            {
                var sql = "SELECT Type FROM CurrencyType WHERE Id = @currencyId";
                return await _connection.ExecuteScalarAsync<string>(sql, new { currencyId }, _transaction);
            }

            return "Unknown";
        }


        public async Task<Account> FindAccountByIBANAsync(string IBAN)
        {
            Account account = null;
            if (_connection != null && _transaction != null)
            {
                var sql = "SELECT * FROM Account WHERE IBAN = @IBAN";
                account = await _connection.QueryFirstOrDefaultAsync<Account>(sql, new { IBAN }, _transaction);
            }
            return account;
        }

        public async Task<Account> FindAccountByIBANandEmailAsync(string IBAN, string email)
        {
            Account account = null;
            if (_connection != null && _transaction != null)
            {
                var sql = "SELECT TOP 1 * FROM Account WHERE PersonId = "+
                    "(SELECT Id FROM Person WHERE Email = @email) AND IBAN = @IBAN";
                account = await _connection.QueryFirstOrDefaultAsync<Account>(sql, new { email, IBAN }, _transaction);
            }
            return account;
        }

        public async Task<bool> UpdateAccountAmountAsync(int id, decimal amount)
        {
            bool updated = false;
            if (_connection != null && _transaction != null)
            {
                var sql = "UPDATE Account SET Amount = Amount + @amount where Id = @id";
                var rowsAffected = await _connection.ExecuteAsync(sql, new { id,amount }, _transaction);
                updated = rowsAffected > 0;
            }
            return updated;
        }

    
    }
}
