using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbCreation.Helpers
{
    public class DbConnectionFactory: IDbConnectionFactory
    {
        private readonly string _serverConnectionString;
        private readonly string _dbConnectionString;

        public DbConnectionFactory(string serverConnectionString, string dbConnectionString)
        {
            _serverConnectionString = serverConnectionString;
            _dbConnectionString = dbConnectionString;
        }

        public IDbConnection CreateServerConnection()
        {
            return new SqlConnection(_serverConnectionString);
        }
        public IDbConnection CreateDbConnection()
        {
            return new SqlConnection(_dbConnectionString);
        }
    }
}
