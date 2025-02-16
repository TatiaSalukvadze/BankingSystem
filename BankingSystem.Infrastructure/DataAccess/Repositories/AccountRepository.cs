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
    }
}
