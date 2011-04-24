using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NBlog.Web.Application.Storage
{
    public interface IRepository
    {
        TEntity Single<TEntity, TKey>(TKey key) where TEntity : new();
        IEnumerable<TEntity> All<TEntity>() where TEntity : new();
        bool Exists<TEntity>(string key) where TEntity : class, new();
        bool Exists<TEntity, TKey>(TKey key) where TEntity : new();
        void Save<TEntity>(TEntity item) where TEntity : class, new();
        void Delete<TEntity, TKey>(TKey key);
    }
}
