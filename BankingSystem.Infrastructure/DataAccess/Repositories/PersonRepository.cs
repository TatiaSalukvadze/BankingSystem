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
        public async Task RegisterPersonAsync(Person person)
        {

            if (_connection != null && _transaction != null)
            {
                var sql = "INSERT INTO Books(Title,Author,YearPublished) VALUES (@Title,@Author,@YearPublished)";
                _connection.Execute(sql, new {Title = "tit"}, _transaction);
                

            }
        }
    }
}
