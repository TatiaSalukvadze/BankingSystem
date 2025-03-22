using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IRepositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace BankingSystem.Infrastructure.DataAccess
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private IServiceProvider _serviceProvider;
        public IDbTransaction Transaction() => _transaction;

        public UnitOfWork(SqlConnection connection, IServiceProvider serviceProvider)
        {
            _connection = connection;
            _connection.Open();
            _serviceProvider = serviceProvider;
        }

        public IPersonRepository PersonRepository => _serviceProvider.GetRequiredService<IPersonRepository>();
        public IAccountRepository AccountRepository => _serviceProvider.GetRequiredService<IAccountRepository>();
        public ICardRepository CardRepository => _serviceProvider.GetRequiredService<ICardRepository>();
        public ITransactionDetailsRepository TransactionDetailsRepository => _serviceProvider.GetRequiredService<ITransactionDetailsRepository>();

        public void BeginTransaction()
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            _transaction = (SqlTransaction)_connection.BeginTransaction();
        }

        public void SaveChanges()
        {
            if (_transaction == null)
                throw new InvalidOperationException
                 ("Transaction have already been committed. Check your transaction handling.");

            _transaction.Commit();
            _transaction = null;
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction = null;
            }

            if (_connection != null && _connection.State != ConnectionState.Closed)
            {
                _connection.Close();
                _connection = null;
            }
        }
    }
}

