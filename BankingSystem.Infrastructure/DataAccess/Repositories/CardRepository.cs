using BankingSystem.Contracts.Interfaces.IRepositories;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BankingSystem.Infrastructure.DataAccess.Repositories
{
    public class CardRepository : ICardRepository
    {
        private IDbConnection _connection;
        private SqlTransaction _transaction;

        public void GiveCommandData(IDbConnection connection, SqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }
    }
}
