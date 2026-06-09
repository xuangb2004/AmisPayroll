using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AmisPayroll.Application.Interfaces.Repositories;
using AmisPayroll.Entities.Base;
using AmisPayroll.Infrastructure.Context;
using Dapper;

namespace AmisPayroll.Infrastructure.Repositories
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly IDbConnectionFactory _connectionFactory;
        protected string _tableName = "";
        protected string _primaryKeyColumn = "";

        public BaseRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<TEntity>($"SELECT * FROM {_tableName}");
        }

        public virtual async Task<TEntity> GetByIdAsync(Guid id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = $"SELECT * FROM {_tableName} WHERE {_primaryKeyColumn} = @Id";
            return await connection.QueryFirstOrDefaultAsync<TEntity>(sql, new { Id = id });
        }

        public virtual async Task<int> DeleteAsync(Guid id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = $"DELETE FROM {_tableName} WHERE {_primaryKeyColumn} = @Id";
            return await connection.ExecuteAsync(sql, new { Id = id });
        }

        public abstract Task<int> InsertAsync(TEntity entity);
        public abstract Task<int> UpdateAsync(TEntity entity);
    }
}