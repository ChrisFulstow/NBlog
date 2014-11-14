using NBlog.Web.Application.Service.Entity;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

namespace NBlog.Web.Application.Storage.Sql
{
	public class SqlRepository : IRepository
	{
		private readonly RepositoryKeys _keys;
		private readonly Database _db;
		private readonly string _serverConnectionString;
		private readonly string _databaseName;


		public SqlRepository(RepositoryKeys keys, string connectionString, string databaseName)
		{
			_keys = keys;
			_serverConnectionString = connectionString;
			_databaseName = databaseName;

			AssertValidDatabaseName();

			if (!DatabaseExists())
			{
				CreateDatabase();
			}

			_db = new Database(GetDatabaseConnectionString(), "System.Data.SqlClient");
		}


		public TEntity Single<TEntity>(object key) where TEntity : class, new()
		{
			var keyName = _keys.GetKeyName<TEntity>();
			TEntity entity = null;
			// Custom query that traverses multiple tables for the Config settings due to the model schema
			if (typeof(TEntity).Equals(typeof(Config)))
			{
				entity = _db.Query<TEntity, Config.CloudConfig, Config.ContactFormConfig, Config.DisqusConfig>(
					string.Format(@"SELECT Config.*, Cloud.*, ContactForm.*, Disqus.* FROM Config
					INNER JOIN Cloud ON Config.CloudId = Cloud.Id
					INNER JOIN ContactForm ON Config.ContactFormId = ContactForm.Id
					INNER JOIN Disqus ON Config.DisqusId = Disqus.Id
					WHERE [{0}] = @0", keyName), key).SingleOrDefault();
			}
			else
			{
				entity = _db.Single<TEntity>(string.Format("WHERE [{0}] = @0", keyName), key);
			}
			return entity;
		}


		public void Save<TEntity>(TEntity item) where TEntity : class, new()
		{
			var keyValue = _keys.GetKeyValue(item).ToString();

			if (Exists<TEntity>(keyValue))
			{
				_db.Update(item);
			}
			else
			{
				_db.Insert(item);
			}
		}


		public IEnumerable<TEntity> All<TEntity>() where TEntity : class, new()
		{
			return _db.Query<TEntity>("SELECT * FROM " + GetTableName<TEntity>());
		}


		public bool Exists<TEntity>(object key) where TEntity : class, new()
		{
			var pkName = _keys.GetKeyName<TEntity>();
			var entity = _db.SingleOrDefault<TEntity>(string.Format("WHERE [{0}] = @0", pkName), key);
			return (entity != null);
		}


		public void Delete<TEntity>(object key) where TEntity : class, new()
		{
			var tableName = GetTableName<TEntity>();
			var keyName = _keys.GetKeyName<TEntity>();
			_db.Execute(string.Format("DELETE FROM {0} WHERE [{1}] = @0", tableName, keyName), key);
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


		private void AssertValidDatabaseName()
		{
			const string databaseNamePattern = @"[a-z0-9_]+";
			if (!Regex.IsMatch(_databaseName, databaseNamePattern, RegexOptions.IgnoreCase))
			{
				throw new Exception("Invalid database name '" + "', must match " + databaseNamePattern);
			}
		}


		private string GetDatabaseConnectionString()
		{
			var builder = new SqlConnectionStringBuilder(_serverConnectionString)
			{
				InitialCatalog = _databaseName,
				ApplicationName = "NBlog"
			};

			return builder.ToString();
		}
	}
}