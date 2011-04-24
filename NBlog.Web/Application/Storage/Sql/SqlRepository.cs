using System;
using System.Linq;

namespace NBlog.Web.Application.Storage.Sql
{
    public class SqlRepository : IRepository
    {
        public TEntity Single<TEntity, TKey>(TKey key)
        {
            throw new NotImplementedException();
        }

        public void Save<TEntity>(TEntity item)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> All<TEntity>()
        {
            throw new NotImplementedException();
        }

        public bool Exists<TEntity>(string key)
        {
            throw new NotImplementedException();
        }

        public bool Exists<TEntity, TKey>(TKey key)
        {
            throw new NotImplementedException();
        }

        public void Delete<TEntity, TKey>(TKey key)
        {
            throw new NotImplementedException();
        }
    }
}