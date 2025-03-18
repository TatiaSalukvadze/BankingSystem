using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IRepositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace BankingSystem.Infrastructure.DataAccess
{
    #region stackOverflowVersion
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private IServiceProvider _serviceProvider;
        public IDbTransaction Transaction() => _transaction;
        public UnitOfWork(SqlConnection connection, IServiceProvider _serviceProvider)
        {
            _connection = connection;
            _connection.Open();
            this._serviceProvider = _serviceProvider;
        }
        //private IPersonRepository _personRepository;
        //private IAccountRepository _accountRepository;
        //private ICardRepository _cardRepository;
        //private ITransactionDetailsRepository _transactionDetailsRepository;
        //private T InitService<T>(ref T member)
        //{
        //    return member ??= _serviceProvider.GetService<T>();
        //}
        public IPersonRepository PersonRepository => _serviceProvider.GetRequiredService<IPersonRepository>();// InitService(ref _personRepository);
        public IAccountRepository AccountRepository => _serviceProvider.GetRequiredService<IAccountRepository>();// InitService(ref _accountRepository); 
        public ICardRepository CardRepository => _serviceProvider.GetRequiredService<ICardRepository>();//  InitService(ref _cardRepository);
        public ITransactionDetailsRepository TransactionDetailsRepository => _serviceProvider.GetRequiredService<ITransactionDetailsRepository>();//InitService(ref _transactionDetailsRepository); // 



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
    #endregion

    #region LazyVersion
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private IServiceProvider _serviceProvider;
        public IDbTransaction Transaction() => _transaction;
        public UnitOfWork(SqlConnection connection, IServiceProvider _serviceProvider)
        {
            _connection = connection;
            _connection.Open();
            this._serviceProvider = _serviceProvider;
            _personRepository = new Lazy<IPersonRepository>(() => _serviceProvider.GetRequiredService<IPersonRepository>());
            _accountRepository = new Lazy<IAccountRepository>(() => _serviceProvider.GetRequiredService<IAccountRepository>());
            _cardRepository = new Lazy<ICardRepository>(() => _serviceProvider.GetRequiredService<ICardRepository>());
            _transactionDetailsRepository = new Lazy<ITransactionDetailsRepository>(() => _serviceProvider.GetRequiredService<ITransactionDetailsRepository>());

        }
        private Lazy<IPersonRepository> _personRepository;
        private Lazy<IAccountRepository> _accountRepository;
        private Lazy<ICardRepository> _cardRepository;
        private Lazy<ITransactionDetailsRepository> _transactionDetailsRepository;

        public IPersonRepository PersonRepository => _personRepository.Value;
        public IAccountRepository AccountRepository => _accountRepository.Value;
        public ICardRepository CardRepository => _cardRepository.Value;
        public ITransactionDetailsRepository TransactionDetailsRepository => _transactionDetailsRepository.Value;

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
    #endregion
}

