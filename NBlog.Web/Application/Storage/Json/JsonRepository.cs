using System;
using System.Collections.Generic;
using System.IO;
using NBlog.Web.Application.Service.Entity;
using Newtonsoft.Json;

namespace NBlog.Web.Application.Storage.Json
{
    public class JsonRepository : IRepository
    {
        private readonly RepositoryKeys _keys;
        private readonly string _dataPath;

        public JsonRepository(RepositoryKeys keys, string dataPath)
        {
            _keys = keys;
            _dataPath = dataPath;
        }


        public string DataPath { get { return _dataPath; } }


        public TEntity Single<TEntity>(string key) where TEntity : class, new()
        {
            return Single<TEntity, string>(key);
        }


        public TEntity Single<TEntity, TKey>(TKey key) where TEntity : class, new()
        {
            var filename = key.ToString();
            var recordPath = Path.Combine(_dataPath, typeof(TEntity).Name, filename + ".json");
            var json = File.ReadAllText(recordPath);
            var item = JsonConvert.DeserializeObject<TEntity>(json);
            return item;
        }


        public IEnumerable<TEntity> All<TEntity>() where TEntity : class, new()
        {
            var folderPath = Path.Combine(_dataPath, typeof(TEntity).Name);
            var filePaths = Directory.GetFiles(folderPath, "*.json", SearchOption.TopDirectoryOnly);

            var list = new List<TEntity>();
            foreach (var path in filePaths)
            {
                var jsonString = File.ReadAllText(path);
                var entity = JsonConvert.DeserializeObject<TEntity>(jsonString);
                list.Add(entity);
            }

            return list;
        }


        public void Save<TEntity>(TEntity item) where TEntity : class, new()
        {
            var json = JsonConvert.SerializeObject(item, Formatting.Indented);
            var folderPath = GetEntityPath<TEntity>();
            if (!Directory.Exists(folderPath)) { Directory.CreateDirectory(folderPath); }

            var filename = _keys.GetKeyValue(item);
            var recordPath = Path.Combine(folderPath, filename + ".json");

            File.WriteAllText(recordPath, json);
        }


        public bool Exists<TEntity>(object key) where TEntity : class, new()
        {
            var folderPath = GetEntityPath<TEntity>();
            var recordPath = Path.Combine(folderPath, key + ".json");
            return File.Exists(recordPath);
        }


        public void DeleteAll<TEntity>()
        {
            var folderPath = GetEntityPath<TEntity>();
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
            }
        }

        public void Delete<TEntity, TKey>(TKey key)
        {
            var folderPath = GetEntityPath<TEntity>();
            var recordPath = Path.Combine(folderPath, key + ".json");
            File.Delete(recordPath);
        }


        // todo: need a TEntity + TKey version of this too that does Path.Combine(folderPath, key + ".json");
        private string GetEntityPath<TEntity>()
        {
            return Path.Combine(_dataPath, typeof(TEntity).Name);
        }
    }
}
