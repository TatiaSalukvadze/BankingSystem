﻿using BankingSystem.Contracts.DTOs.UserBanking;
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

        public async Task<Card> GetCardAsync(string cardNumber)
        {
            Card card = null;
            if (_connection != null)
            {
                var sql = "SELECT TOP 1 * FROM Card WHERE CardNumber = @cardNumber";
                card = await _connection.QueryFirstOrDefaultAsync<Card>(sql, new { cardNumber });
            }
            return card;
        }

        public async Task<List<CardWithIBANDTO>> GetCardsForPersonAsync(string email)
        {
            var result = new List<CardWithIBANDTO> { };
            if (_connection != null)
            {
                var sql = @"SELECT a.IBAN, p.[Name], p.Surname, c.CardNumber, c.ExpirationDate, c.CVV, c.PIN 
                    FROM Card AS c JOIN Account AS a ON c.AccountId = a.Id
                    JOIN Person AS p ON a.PersonId = p.Id 
                    WHERE @email = p.Email";
                result = (await _connection.QueryAsync<CardWithIBANDTO>(sql, new { email })).ToList();
            }
            return result;
        }

        public async Task<bool> CardNumberExistsAsync(string cardNumber)
        {
            bool exists = false;
            if (_connection != null)
            {
                var sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM Card WHERE CardNumber = @CardNumber) THEN 1 ELSE 0 END";
                exists = await _connection.ExecuteScalarAsync<bool>(sql, new { CardNumber = cardNumber });
            }
            return exists;
        }

        public async Task<int> CreateCardAsync(Card card)
        {
            int insertedId = 0;
            if (_connection != null && _transaction != null)
            {
                var sql = @"INSERT INTO Card (AccountId, CardNumber, ExpirationDate, CVV, PIN) 
                          OUTPUT INSERTED.Id
                          VALUES (@AccountId, @CardNumber, @ExpirationDate, @CVV, @PIN)";
                insertedId = await _connection.ExecuteScalarAsync<int>(sql, card, _transaction);
            }
            return insertedId;
        }

        public async Task<bool> UpdateCardAsync(int cardId, string newPIN)
        {
            bool updated = false;
            if (_connection != null && _transaction != null)
            {
                var sql = "UPDATE Card SET PIN = @newPIN WHERE Id = @cardId";
                int rowsAffected = await _connection.ExecuteAsync(sql, new { cardId, newPIN }, _transaction);
                if(rowsAffected > 0) { updated = true; }
            }
            return updated;
        }
        
        public async Task<bool> DeleteCardAsync(string cardNumber)
        {
            bool deleted = false;
            if (_connection != null && _transaction != null)
            {
                var sql = "DELETE FROM Card WHERE CardNumber = @cardNumber";
                int rowsAffected = await _connection.ExecuteAsync(sql, new { cardNumber }, _transaction);
                if (rowsAffected > 0) { deleted = true; }
            }
            return deleted;
        }
    }
}
