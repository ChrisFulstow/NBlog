using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using NBlog.Web.Application.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NBlog.Web.Application.Storage.Azure
{
	public class AzureBlobRepository : IRepository
	{
		private readonly RepositoryKeys _keys;
		private readonly HttpTenantSelector _tenantSelector;
		private readonly CloudBlobContainer _container;

		public AzureBlobRepository(RepositoryKeys keys, HttpTenantSelector tenantSelector)
		{
			_keys = keys;
			_tenantSelector = tenantSelector;

			var storage = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("AzureBlobRepository.ConnectionString"));
			var blobClient = storage.CreateCloudBlobClient();
			string name = GetContainerSafeName(tenantSelector);
			_container = blobClient.GetContainerReference(name);
		}

		private string GetContainerSafeName(HttpTenantSelector selector)
		{
			string name = selector.Name;
			if (name.Any(c => Char.IsNumber(c)))
				name = "localhost";
			return name;
		}

		private string GetItemPath<TEntity>(string key)
		{
			return String.Format("{0}/{1}.json", typeof(TEntity).Name, key);
		}

		public TEntity Single<TEntity>(object key) where TEntity : class, new()
		{
			string relativePath = GetItemPath<TEntity>(key.ToString());
			ICloudBlob blob = _container.GetBlockBlobReference(relativePath);
			if (!blob.Exists())
			{
				throw new FileNotFoundException("The item '" + relativePath + "' could not be found. Container: " + _container.Name + " " + _container.Uri);
			}
			string json = blob.DownloadText();
			var item = JsonConvert.DeserializeObject<TEntity>(json);
			return item;
		}

		public IEnumerable<TEntity> All<TEntity>() where TEntity : class, new()
		{
			var list = new List<TEntity>();
			var directory = _container.GetDirectoryReference(typeof(TEntity).Name);
			var blobs = directory.ListBlobs();

			foreach (var blob in blobs)
			{
				string relativePath = string.Format("{0}{1}", blob.Parent.Prefix, blob.Uri.Segments.LastOrDefault());
				ICloudBlob b = _container.GetBlockBlobReference(relativePath);
				string json = b.DownloadText();

				var entity = JsonConvert.DeserializeObject<TEntity>(json);
				list.Add(entity);
			}

			return list;
		}

		public bool Exists<TEntity>(object key) where TEntity : class, new()
		{
			string relativePath = GetItemPath<TEntity>(key.ToString());
			ICloudBlob blob = _container.GetBlockBlobReference(relativePath);
			return blob.Exists();
		}

		public void Save<TEntity>(TEntity item) where TEntity : class, new()
		{
			var json = JsonConvert.SerializeObject(item, Formatting.Indented);
			var key = _keys.GetKeyValue(item).ToString();
			string relativePath = GetItemPath<TEntity>(key);
			ICloudBlob blob = _container.GetBlockBlobReference(relativePath);
			blob.UploadText(json);
		}

		public void Delete<TEntity>(object key) where TEntity : class, new()
		{
			string relativePath = GetItemPath<TEntity>(key.ToString());
			ICloudBlob blob = _container.GetBlockBlobReference(relativePath);
			blob.Delete();
		}
	}
}