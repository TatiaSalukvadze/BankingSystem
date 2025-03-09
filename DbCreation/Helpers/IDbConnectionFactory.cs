using System.Data;

namespace DbCreation.Helpers
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateServerConnection();
        IDbConnection CreateDbConnection();
    }
}
