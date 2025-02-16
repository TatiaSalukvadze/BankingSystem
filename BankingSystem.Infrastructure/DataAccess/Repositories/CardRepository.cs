using BankingSystem.Contracts.Interfaces.IRepositories;
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
    }
}
