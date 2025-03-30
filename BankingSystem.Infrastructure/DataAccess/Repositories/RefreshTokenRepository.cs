﻿using BankingSystem.Contracts.Interfaces.IRepositories;
using BankingSystem.Domain.Entities;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace BankingSystem.Infrastructure.DataAccess.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private SqlConnection _connection;
        private IDbTransaction _transaction;

        public RefreshTokenRepository(SqlConnection connection)
        {
            _connection = connection;
        }

        public void SetTransaction(IDbTransaction transaction)
        {
            _transaction = transaction;
        }
        public async Task<bool> SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            int addedTokenId = 0;
            if (_connection != null)
            {
                var sql = @"INSERT INTO RefreshToken(IdentityUserId, Token, ExpirationDate, DeviceId, CreatedAt)
                     OUTPUT INSERTED.Id
                     VALUES (@IdentityUserId, @Token, @ExpirationDate, @DeviceId, @CreatedAt)";
                addedTokenId = await _connection.ExecuteScalarAsync<int>(sql, refreshToken);
            }
            return addedTokenId > 0? true : false;
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string token)
        {
            if (_connection != null)
            {
                var sql = "SELECT TOP 1 * FROM RefreshToken WHERE Token = @token";
                return await _connection.QueryFirstOrDefaultAsync<RefreshToken>(sql, new { token });
            }
            return null;
        }

        public async Task<bool> DeleteRefreshTokensync(int id)
        {
            bool deleted = false;
            if (_connection != null)
            {
                var sql = "DELETE FROM RefreshToken WHERE Id = @id";
                var rowsAffected = await _connection.ExecuteAsync(sql, new { id });
                deleted = rowsAffected > 0;
            }
            return deleted;
        }

    }
}
