using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IRepositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace BankingSystem.Infrastructure.DataAccess
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        public IDbTransaction Transaction() => _transaction;

        public UnitOfWork(SqlConnection connection, IPersonRepository personRepository, 
            IAccountRepository accountRepository, ICardRepository cardRepository, 
            ITransactionDetailsRepository transactionDetailsRepository)//IServiceProvider _serviceProvider) 
        {
            _connection = connection;
            _connection.Open();
            PersonRepository = personRepository;
            AccountRepository = accountRepository;
            CardRepository = cardRepository;
            TransactionDetailsRepository = transactionDetailsRepository;
            //_personRepository = new Lazy<IPersonRepository>(() => _serviceProvider.GetRequiredService<IPersonRepository>());
            //_accountRepository = new Lazy<IAccountRepository>(() => _serviceProvider.GetRequiredService<IAccountRepository>());
            //_cardRepository = new Lazy<ICardRepository>(() => _serviceProvider.GetRequiredService<ICardRepository>());
            //_transactionDetailsRepository = new Lazy<ITransactionDetailsRepository>(() => _serviceProvider.GetRequiredService<ITransactionDetailsRepository>());

        }
        //private Lazy<IPersonRepository> _personRepository;
        //private Lazy<IAccountRepository> _accountRepository;
        //private Lazy<ICardRepository> _cardRepository;
        //private Lazy<ITransactionDetailsRepository> _transactionDetailsRepository;

        //public IPersonRepository PersonRepository => _personRepository.Value;
        //public IAccountRepository AccountRepository => _accountRepository.Value;
        //public ICardRepository CardRepository => _cardRepository.Value;
        //public ITransactionDetailsRepository TransactionDetailsRepository => _transactionDetailsRepository.Value;

        private IPersonRepository _personRepository;
        public IPersonRepository PersonRepository
        {
            get
            {
                return _personRepository;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("There is no person repository present");
                }
                else
                {
                    _personRepository = value;
                }
            }
        }

        private IAccountRepository _accountRepository;
        public IAccountRepository AccountRepository
        {
            get
            {
                return _accountRepository;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("There is no account repository present");
                }
                else
                {
                    _accountRepository = value;
                }
            }
        }

        private ICardRepository _cardRepository;
        public ICardRepository CardRepository
        {
            get
            {
                return _cardRepository;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("There is no card repository present");
                }
                else
                {
                    _cardRepository = value;
                }
            }
        }

        private ITransactionDetailsRepository _transactionDetailsRepository;
        public ITransactionDetailsRepository TransactionDetailsRepository
        {
            get
            {
                return _transactionDetailsRepository;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("There is no transactionDetails repository present");
                }
                else
                {
                    _transactionDetailsRepository = value;
                }
            }
        }

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

