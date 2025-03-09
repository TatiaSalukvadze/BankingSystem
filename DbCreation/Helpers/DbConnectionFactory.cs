using Microsoft.Data.SqlClient;
using System.Data;

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
