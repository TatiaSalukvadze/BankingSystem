using BankingSystem.Contracts.Interfaces.IRepositories;
using BankingSystem.Domain.Entities;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using static Dapper.SqlMapper;

namespace BankingSystem.Infrastructure.DataAccess.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private IDbConnection _connection;
        private SqlTransaction _transaction;

        public void GiveCommandData(IDbConnection connection,SqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<Person?> FindByIdentityIdAsync(string identityId)
        {
            if (_connection != null && _transaction != null)
            {
                var sql = "SELECT * FROM Person WHERE IdentityUserId = @identityId";
                return await _connection.QueryFirstOrDefaultAsync<Person>(sql, new { identityId }, _transaction);
            }

            return null;
        }

        public async Task<int> FindIdByIDNumberAsync(string IDNumber)
        {
            int id = 0;
            if (_connection != null && _transaction != null)
            {
                var sql = "SELECT Id FROM Person WHERE IDNumber = @IDNumber";
                id = await _connection.ExecuteScalarAsync<int>(sql, new { IDNumber }, _transaction);
            }

            return id;
        }

        public async Task<int> RegisterPersonAsync(Person person)
        {
            int addedUserId = 0;
            if (_connection != null && _transaction != null)
            {
                var sql = "INSERT INTO Person(IdentityUserId, [Name], Surname, IDNumber, Birthdate, Email)" +
                    " OUTPUT INSERTED.Id" +
                    " VALUES (@IdentityUserId,@Name, @Surname, @IDNumber, @Birthdate, @Email)";
                addedUserId =  await _connection.ExecuteScalarAsync<int>(sql, person, _transaction);
            }
            return addedUserId;
        }


    }
}
