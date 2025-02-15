using BankingSystem.Contracts.Interfaces;
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
                var sql = "SELECT * FROM Persons WHERE IdentityUserId = @identityId";
                return await _connection.QueryFirstOrDefaultAsync<Person>(sql, new { identityId });
            }

            return null;
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
