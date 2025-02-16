using BankingSystem.Contracts.Interfaces.IRepositories;
using BankingSystem.Domain.Entities;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BankingSystem.Infrastructure.DataAccess.Repositories
{
    public class CardRepository : ICardRepository
    {
        private SqlConnection _connection;
        private IDbTransaction _transaction;

        public CardRepository(SqlConnection connection, IDbTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        //public async Task<bool> AccountExists(int accountId)
        //{
        //    bool exists = false;
        //    if (_connection != null && _transaction != null)
        //    {
        //        var sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM Account WHERE Id = @AccountId) THEN 1 ELSE 0 END";
        //        exists = await _connection.ExecuteScalarAsync<bool>(sql, new { AccountId = accountId }, _transaction);
        //    }
        //    return exists;
        //}

        public async Task<bool> CardNumberExists(string cardNumber)
        {
            bool exists = false;
            if (_connection != null && _transaction != null)
            {
                var sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM Card WHERE CardNumber = @CardNumber) THEN 1 ELSE 0 END";
                exists = await _connection.ExecuteScalarAsync<bool>(sql, new { CardNumber = cardNumber }, _transaction);
            }
            return exists;
        }

        public async Task<int> CreateCardAsync(Card card)
        {
            int insertedId = 0;
            if (_connection != null && _transaction != null)
            {
                var sql = "INSERT INTO Card (AccountId, CardNumber, ExpirationDate, CVV, PIN) " +
                          "OUTPUT INSERTED.Id " +
                          "VALUES (@AccountId, @CardNumber, @ExpirationDate, @CVV, @PIN)";
                insertedId = await _connection.ExecuteScalarAsync<int>(sql, card, _transaction);
            }
            return insertedId;
        }
    }
}
