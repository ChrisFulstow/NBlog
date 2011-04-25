using System;
using System.Collections.Generic;

namespace NBlog.Web.Application.Storage.Mongo
{
    public class MongoRepository : IRepository
    {
        private readonly RepositoryKeys _keys;
        private readonly string _connectionString;
        private readonly string _databaseName;


        public MongoRepository(RepositoryKeys keys, string connectionString, string databaseName)
        {
            _keys = keys;
            _connectionString = connectionString;
            _databaseName = databaseName;
        }


        public TEntity Single<TEntity>(object key) where TEntity : class, new()
        {
            throw new NotImplementedException();
        }


        public IEnumerable<TEntity> All<TEntity>() where TEntity : class, new()
        {
            throw new NotImplementedException();
        }


        public bool Exists<TEntity>(object key) where TEntity : class, new()
        {
            throw new NotImplementedException();
        }


        public void Save<TEntity>(TEntity item) where TEntity : class, new()
        {
            throw new NotImplementedException();
        }

        public void Delete<TEntity>(object key) where TEntity : class, new()
        {
            throw new NotImplementedException();
        }
    }
}