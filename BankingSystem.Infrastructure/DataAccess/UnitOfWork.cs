using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IRepositories;
using BankingSystem.Domain.Entities;
using BankingSystem.Infrastructure.DataAccess.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Infrastructure.DataAccess
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private string _connectionString;
        private IDbConnection _connection;
        private SqlTransaction _transaction;
        public SqlTransaction Transaction() => _transaction;//no nned
        public IDbConnection Connection() => _connection;//no need

        public UnitOfWork(IConfiguration configuration, IPersonRepository personRepository)
        {
            
            _connectionString = configuration.GetConnectionString("default") ??
                throw new ArgumentNullException("There is no default connection string present");
            _connection = new SqlConnection(_connectionString);
            _connection.Open();
            _transaction = (SqlTransaction)_connection.BeginTransaction();
            RepoSetUp(personRepository);
            //PersonRepository = personRepository;
        }
        private void RepoSetUp(IPersonRepository personRepository)
        {
            PersonRepository = personRepository;
            PersonRepository.GiveCommandData(_connection, _transaction);
        }
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

        //public void BeginTransaction()
        //{
        //    if (_connection.State != ConnectionState.Open)
        //    {
        //        _connection.Open();  // Open the connection here, when needed
        //    }

        //    _transaction = (SqlTransaction)_connection.BeginTransaction();
        //}

        //public void Rollback() => _transaction.Rollback();

        //public void Dispose()
        //{
        //    _transaction?.Dispose();
        //    _connection?.Close();
        //}

        public IDbCommand CreateCommand()
        {
            var command = _connection.CreateCommand();
            command.Transaction = _transaction;
            return command;
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

