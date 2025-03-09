using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbCreation.Helpers
{
    public class DbConnectionFactory
    {
        private readonly string _masterConnectionString;
        private readonly string _dbConnectionString;

        public DbConnectionFactory(string masterConnectionString, string dbConnectionString)
        {
            _masterConnectionString = masterConnectionString;
            _dbConnectionString = dbConnectionString;
        }
    }
}
