using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IRepositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BankingSystem.Infrastructure.DataAccess
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        //private string _connectionString;
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        public IDbTransaction Transaction() => _transaction;//no nned
        //public IDbConnection Connection() => _connection;//no need

        public UnitOfWork(SqlConnection connection, IPersonRepository personRepository, 
            IAccountRepository accountRepository, ICardRepository cardRepository, 
            ITransactionDetailsRepository transactionDetailsRepository)
        {

            //_connectionString = configuration.GetConnectionString("default") ??
            //    throw new ArgumentNullException("There is no default connection string present");
            //_connection = new SqlConnection(_connectionString);
            //_connection.Open();
            //_transaction = dbTransaction;
            _connection = connection;// _transaction.Connection;
            _connection.Open();
            //RepoSetUp(personRepository, accountRepository, cardRepository, transactionDetailsRepository);
            PersonRepository = personRepository;
            AccountRepository = accountRepository;
            CardRepository = cardRepository;
            TransactionDetailsRepository = transactionDetailsRepository;
        }
        //private void RepoSetUp(IPersonRepository personRepository, IAccountRepository accountRepository,
        //   ICardRepository cardRepository,ITransactionDetailsRepository transactionDetailsRepository)
        //{
        //    PersonRepository = personRepository;
        //    PersonRepository.GiveCommandData(_connection, _transaction);
        //    AccountRepository = accountRepository;
        //    AccountRepository.GiveCommandData(_connection, _transaction);
        //    CardRepository = cardRepository;
        //    CardRepository.GiveCommandData(_connection, _transaction);
        //    TransactionDetailsRepository = transactionDetailsRepository;    
        //    TransactionDetailsRepository.GiveCommandData(_connection, _transaction);
        //}

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
                _connection.Open();  // Open the connection here, when needed
            }

            _transaction = (SqlTransaction)_connection.BeginTransaction();
        }

        //public void Rollback() => _transaction.Rollback();

        //public void Dispose()
        //{
        //    _transaction?.Dispose();
        //    _connection?.Close();
        //}

        //public IDbCommand CreateCommand()
        //{
        //    var command = _connection.CreateCommand();
        //    command.Transaction = _transaction;
        //    return command;
        //}

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

