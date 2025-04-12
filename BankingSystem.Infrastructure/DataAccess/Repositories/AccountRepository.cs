using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Contracts.Interfaces.IRepositories;
using BankingSystem.Domain.Entities;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BankingSystem.Infrastructure.DataAccess.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IDbConnection _connection;
        private IDbTransaction _transaction;

        public AccountRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public void SetTransaction(IDbTransaction transaction)
        {
            _transaction = transaction;
        }

        public async Task<int> CreateAccountAsync(Account account)
        {
            int insertedId = 0;
            if (_connection != null)
            {
                var sql = @"INSERT INTO Account (PersonId, IBAN, Amount, Currency) 
                    OUTPUT INSERTED.Id 
                    VALUES (@PersonId, @IBAN, @Amount, @Currency)";
                insertedId = await _connection.ExecuteScalarAsync<int>(sql, account);
            }
            return insertedId;
        }

        public async Task<bool> IBANExistsAsync(string IBAN)
        {
            bool exists = false;
            if (_connection != null)
            {
                var sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM Account WHERE IBAN = @IBAN) THEN 1 ELSE 0 END AS IBANExists";
                exists = await _connection.ExecuteScalarAsync<bool>(sql, new {IBAN});
            }
            return exists;
        }

        public async Task<bool> AccountExistForEmailAsync(string email)
        {
            bool exists = false;
            if (_connection != null)
            {
                var sql = @"SELECT CASE WHEN EXISTS (SELECT 1 FROM Account as a WHERE @email = 
                    (SELECT TOP 1 Email FROM Person WHERE Id = a.PersonId)) THEN 1 ELSE 0 END AS AccountExists";
                exists = await _connection.ExecuteScalarAsync<bool>(sql, new { email });
            }
            return exists;
        }
        public async Task<int> AccountsCountForEmailAsync(string email)
        {
            int count = 0;
            if (_connection != null)
            {
                var sql = @"SELECT COUNT(*) FROM Account as a WHERE @email = 
                    (SELECT TOP 1 Email FROM Person WHERE Id = a.PersonId)";
                count = await _connection.ExecuteScalarAsync<int>(sql, new { email });
            }
            return count;
        }
        public async Task<List<SeeAccountsDTO>> SeeAccountsByEmailAsync(string email, int offset, int perPage)
        {
            var result = new List<SeeAccountsDTO>();
            if (_connection != null)
            {
                var sql = @"
                SELECT IBAN, Currency, Amount 
                FROM Account AS a
                WHERE @email = (SELECT TOP 1 Email FROM Person WHERE Id = a.PersonId)
                ORDER BY a.Id DESC
                OFFSET @offset ROW
                FETCH NEXT @perPage ROWS ONLY";
                result = (await _connection.QueryAsync<SeeAccountsDTO>(sql, new { email, offset, perPage })).ToList();
            }
            return result;
        }

        public async Task<Account> FindAccountByIBANAsync(string IBAN)
        {
            Account account = null;
            if (_connection != null)
            {
                var sql = "SELECT TOP 1 * FROM Account WHERE IBAN = @IBAN";
                account = await _connection.QueryFirstOrDefaultAsync<Account>(sql, new { IBAN });
            }
            return account;
        }

        public async Task<Account> FindAccountByIBANandEmailAsync(string IBAN, string email)
        {
            Account account = null;
            if (_connection != null)
            {
                var sql = @"SELECT TOP 1 * FROM Account WHERE PersonId = 
                    (SELECT Id FROM Person WHERE Email = @email) AND IBAN = @IBAN";
                account = await _connection.QueryFirstOrDefaultAsync<Account>(sql, new { email, IBAN });
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

        public async Task<bool> DeleteAccountByIBANAsync(string iban)
        {
            bool deleted = false;
            if (_connection != null)
            {
                var sql = "DELETE FROM Account WHERE IBAN = @IBAN";
                var rowsAffected = await _connection.ExecuteAsync(sql, new { IBAN = iban });
                deleted = rowsAffected > 0;
            }
            return deleted;
        }

        public async Task<decimal> GetBalanceByIBANAsync(string iban)
        {
            if (_connection != null)
            {
                var sql = "SELECT Amount FROM Account WHERE IBAN = @IBAN";
                return await _connection.ExecuteScalarAsync<decimal>(sql, new { IBAN = iban });
            }
            return 0;
        }
    }
}
