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
using static Dapper.SqlMapper;

namespace BankingSystem.Infrastructure.DataAccess.Repositories
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private SqlConnection _connection;
        private IDbTransaction _transaction;

        public CurrencyRepository(SqlConnection connection, IDbTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<int> FindIdByTypeAsync(string type)
        {
            int id = 0;
            if (_connection != null && _transaction != null)
            {
                var sql = "SELECT TOP 1 Id FROM CurrencyType WHERE [Type] = @type";
                id = await _connection.QueryFirstOrDefaultAsync<int>(sql, new { type }, _transaction);
            }

            return id;
        }

        public async Task<string> FindTypeByIdAsync(int id)
        {
            string type = "";
            if (_connection != null && _transaction != null)
            {
                var sql = "SELECT TOP 1 [Type] FROM CurrencyType WHERE Id = @id";
                type = await _connection.QueryFirstOrDefaultAsync<string>(sql, new { id }, _transaction);
            }
            return (type is null)?"":type;
        }
    }
}
