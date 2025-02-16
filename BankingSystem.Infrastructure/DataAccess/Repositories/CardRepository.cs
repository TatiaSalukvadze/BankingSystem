using BankingSystem.Contracts.DTOs;
using BankingSystem.Contracts.Interfaces.IRepositories;
using BankingSystem.Domain.Entities;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
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

        public async Task<List<CardWithIBANDTO>> SeeCardsAsync(string email)
        {
            var result = new List<CardWithIBANDTO> { };
            if (_connection != null && _transaction != null)
            {
                var sql = "SELECT a.IBAN, p.[Name], p.Surname, c.CardNumber, c.ExpirationDate, c.CVV, c.PIN " +
                    "FROM Card AS c JOIN Account AS a ON c.AccountId = a.Id " +
                    "JOIN Person AS p ON a.PersonId = p.Id " +
                    "WHERE @email = p.Email";
                result = (await _connection.QueryAsync<CardWithIBANDTO>(sql, new { email }, _transaction)).ToList();
            }

            return result;
        }
    }
}
