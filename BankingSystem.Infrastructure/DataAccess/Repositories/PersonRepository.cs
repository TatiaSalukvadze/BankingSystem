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
        private SqlConnection _connection;
        private IDbTransaction _transaction;

        public PersonRepository(SqlConnection connection, IDbTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<Person?> FindByIdentityIdAsync(string identityId)
        {
            if (_connection != null)
            {
                var sql = "SELECT TOP 1 * FROM Person WHERE IdentityUserId = @identityId";
                return await _connection.QueryFirstOrDefaultAsync<Person>(sql, new { identityId });
            }
            return null;
        }

        public async Task<int> FindIdByIDNumberAsync(string IDNumber)
        {
            int id = 0;
            if (_connection != null)
            {
                var sql = "SELECT Id FROM Person WHERE IDNumber = @IDNumber";
                id = await _connection.ExecuteScalarAsync<int>(sql, new { IDNumber });
            }
            return id;
        }

        public async Task<int> RegisterPersonAsync(Person person)
        {
            int addedUserId = 0;
            if (_connection != null && _transaction != null)
            {
                var sql = @"INSERT INTO Person(IdentityUserId, [Name], Surname, IDNumber, Birthdate, Email)
                     OUTPUT INSERTED.Id
                     VALUES (@IdentityUserId,@Name, @Surname, @IDNumber, @Birthdate, @Email)";
                addedUserId =  await _connection.ExecuteScalarAsync<int>(sql, person, _transaction);
            }
            return addedUserId;
        }

        public async Task<int> PeopleRegisteredThisYear()
        {
            int count = 0;
            if (_connection != null && _transaction != null)
            {
                var sql = "SELECT COUNT(*) FROM Person WHERE YEAR(CreatedAt) = YEAR(GETDATE())";
                count = await _connection.ExecuteScalarAsync<int>(sql, transaction:_transaction);
            }
            return count;
        }

        public async Task<int> PeopleRegisteredLastOneYear()
        {
            if (_connection != null && _transaction != null)
            {
                var sql = "SELECT COUNT(*) FROM Person WHERE CreatedAt >= DATEADD(YEAR, -1, GETDATE())";
                var count = await _connection.ExecuteScalarAsync<int>(sql, transaction: _transaction);
                return count;
            }
            return 0;
        }

        public async Task<int> PeopleRegisteredLast30Days()
        {
            if (_connection != null && _transaction != null)
            {
                var sql = "SELECT COUNT(*) FROM Person WHERE CreatedAt >= DATEADD(DAY, -30, GETDATE())";
                var count = await _connection.ExecuteScalarAsync<int>(sql, transaction: _transaction);
                return count;
            }
            return 0;
        }
    }
}
