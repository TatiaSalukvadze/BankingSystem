using BankingSystem.Contracts.Interfaces.IRepositories;
using BankingSystem.Domain.Entities;
using Dapper;
using System.Data;
using static Dapper.SqlMapper;

namespace BankingSystem.Infrastructure.DataAccess.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly IDbConnection _connection;
        private IDbTransaction _transaction;

        public PersonRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public void SetTransaction(IDbTransaction transaction)
        {
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
            if (_connection != null)
            {
                var sql = @"INSERT INTO Person(IdentityUserId, [Name], Surname, IDNumber, Birthdate, Email)
                     OUTPUT INSERTED.Id
                     VALUES (@IdentityUserId,@Name, @Surname, @IDNumber, @Birthdate, @Email)";
                addedUserId =  await _connection.ExecuteScalarAsync<int>(sql, person);
            }
            return addedUserId;
        }

        public async Task<int> PeopleRegisteredThisYearAsync()
        {
            int count = 0;
            if (_connection != null)
            {
                var sql = "SELECT COUNT(*) FROM Person WHERE YEAR(CreatedAt) = YEAR(GETDATE())";
                count = await _connection.ExecuteScalarAsync<int>(sql);
            }
            return count;
        }

        public async Task<int> PeopleRegisteredLastOneYearAsync()
        {
            if (_connection != null)
            {
                var sql = "SELECT COUNT(*) FROM Person WHERE CreatedAt >= DATEADD(YEAR, -1, GETDATE())";
                var count = await _connection.ExecuteScalarAsync<int>(sql);
                return count;
            }
            return 0;
        }

        public async Task<int> PeopleRegisteredLast30DaysAsync()
        {
            if (_connection != null)
            {
                var sql = "SELECT COUNT(*) FROM Person WHERE CreatedAt >= DATEADD(DAY, -30, GETDATE())";
                var count = await _connection.ExecuteScalarAsync<int>(sql);
                return count;
            }
            return 0;
        }
    }
}
