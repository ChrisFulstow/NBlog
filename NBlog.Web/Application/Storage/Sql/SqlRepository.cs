using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using PetaPoco;

namespace NBlog.Web.Application.Storage.Sql
{
    public class SqlRepository : IRepository
    {
        private readonly RepositoryKeys _keys;
        private readonly Database _database;
        private readonly string _serverConnectionString;
        private readonly string _databaseName;

        public SqlRepository(RepositoryKeys keys, string connectionString, string databaseName)
        {
            _keys = keys;
            _serverConnectionString = connectionString;
            _databaseName = databaseName;

            const string databaseNamePattern = @"[a-z0-9_]+";
            if (!Regex.IsMatch(databaseName, databaseNamePattern, RegexOptions.IgnoreCase))
            {
                throw new Exception("Invalid database name '" + "', must match " + databaseNamePattern);
            }

            if (!DatabaseExists())
            {
                CreateDatabase();
            }

            var databaseConnectionString = new SqlConnectionStringBuilder(_serverConnectionString)
            {
                InitialCatalog = _databaseName,
                ApplicationName = "NBlog"
            };

            _database = new Database(databaseConnectionString.ToString(), "System.Data.SqlClient");
        }


        public TEntity Single<TEntity, TKey>(TKey key) where TEntity : new()
        {
            var keyName = _keys.GetKeyName<TEntity>();
            var entity = _database.Single<TEntity>(string.Format("WHERE [{0}] = @0", keyName), key);
            return entity;
        }


        public void Save<TEntity>(TEntity item) where TEntity : class, new()
        {
            var keyValue = _keys.GetKeyValue(item).ToString();

            if (Exists<TEntity>(keyValue))
            {
                _database.Update(item);
            }
            else
            {
                _database.Insert(item);
            }
        }


        public IEnumerable<TEntity> All<TEntity>() where TEntity : new()
        {
            return _database.Query<TEntity>("SELECT * FROM " + GetTableName<TEntity>());
        }


        public bool Exists<TEntity>(string key) where TEntity : class, new()
        {
            var pkName = _keys.GetKeyName<TEntity>();
            var entity = _database.SingleOrDefault<TEntity>(string.Format("WHERE [{0}] = @0", pkName), key);
            return (entity != null);
        }


        public bool Exists<TEntity, TKey>(TKey key) where TEntity : new()
        {
            return _database.Exists<TEntity>(key);
        }


        public void Delete<TEntity, TKey>(TKey key)
        {
            _database.Delete(key);
        }


        private void CreateDatabase()
        {
            var createDatabaseSql = string.Format("CREATE DATABASE [{0}]", _databaseName);

            const string createTableEntrySql = @"
                CREATE TABLE [dbo].[Entry]
                (
                    [Id] int NOT NULL IDENTITY (1, 1) PRIMARY KEY,
	                [Slug] [nvarchar](250) NOT NULL,
	                [Title] [nvarchar](250) NULL,
	                [Author] [nvarchar](250) NULL,
	                [DateCreated] [datetime] NULL,
	                [Markdown] [nvarchar](max) NULL,
	                [IsPublished] [bit] NULL,
	                [IsCodePrettified] [bit] NULL
                )";

            using (var cnn = new SqlConnection(_serverConnectionString))
            {
                cnn.Open();

                using (var cmd = new SqlCommand(createDatabaseSql, cnn))
                {
                    cmd.Parameters.AddWithValue("DatabaseName", _databaseName);
                    cmd.ExecuteNonQuery();
                }

                cnn.ChangeDatabase(_databaseName);

                using (var cmd = new SqlCommand(createTableEntrySql, cnn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }


        private bool DatabaseExists()
        {
            try
            {
                const string sql = "SELECT database_id FROM sys.databases WHERE Name = @DatabaseName";

                using (var cnn = new SqlConnection(_serverConnectionString))
                using (var cmd = new SqlCommand(sql, cnn))
                {
                    cnn.Open();
                    cmd.Parameters.AddWithValue("DatabaseName", _databaseName);
                    var databaseId = cmd.ExecuteScalar();
                    var databaseExists = (databaseId != null);
                    return databaseExists;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not connect to SQL Server database '" + _databaseName + "', check your connection string.", ex);
            }
        }


        private static string GetTableName<TEntity>()
        {
            return "[" + typeof(TEntity).Name + "]";
        }
    }
}