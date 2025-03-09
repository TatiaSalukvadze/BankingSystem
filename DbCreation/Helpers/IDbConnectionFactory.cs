using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbCreation.Helpers
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateServerConnection();
        IDbConnection CreateDbConnection();
    }
}
