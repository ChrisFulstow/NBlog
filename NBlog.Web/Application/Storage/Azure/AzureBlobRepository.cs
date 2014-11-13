using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using NBlog.Web.Application.Infrastructure;
using NBlog.Web.Application.Service.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace NBlog.Web.Application.Storage.Azure
{
	public class AzureBlobRepository : IRepository
	{
		private readonly RepositoryKeys _keys;
		private readonly HttpTenantSelector _tenantSelector;
		private readonly CloudBlobContainer _jsonContainer;
		private readonly CloudBlobContainer _imagesContainer;

		public AzureBlobRepository(RepositoryKeys keys, HttpTenantSelector tenantSelector)
		{
			_keys = keys;
			_tenantSelector = tenantSelector;

			var storage = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureBlob"].ConnectionString);
			var blobClient = storage.CreateCloudBlobClient();
			string name = GetContainerSafeName(tenantSelector);
			_jsonContainer = blobClient.GetContainerReference(ConfigurationManager.AppSettings["JsonBlobContainerName"]);
			_jsonContainer.CreateIfNotExists(BlobContainerPublicAccessType.Off);
			_imagesContainer = blobClient.GetContainerReference(ConfigurationManager.AppSettings["ImagesBlobContainerName"]);
			_imagesContainer.CreateIfNotExists(BlobContainerPublicAccessType.Blob);
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
			if (typeof(TEntity).Equals(typeof(Image)))
			{
				return string.Format("{0}/{1}", typeof(TEntity).Name.ToLower(), key);
			}
			else
			{
				return string.Format("{0}/{1}.json", typeof(TEntity).Name.ToLower(), key);
			}
		}

		public TEntity Single<TEntity>(object key) where TEntity : class, new()
		{
			string relativePath = GetItemPath<TEntity>(key.ToString());
			ICloudBlob blob = null;
			if (typeof(TEntity).Equals(typeof(Image)))
			{
				blob = _imagesContainer.GetBlockBlobReference(relativePath);
				if (!blob.Exists())
				{
					throw new FileNotFoundException("The item '" + relativePath + "' could not be found. Container: " + _imagesContainer.Name + " " + _imagesContainer.Uri);
				}
				var image = new Image();
				image.Url = blob.Uri.AbsoluteUri;
				var item = image as TEntity;
				return item;
			}
			else
			{
				blob = _jsonContainer.GetBlockBlobReference(relativePath);
				if (!blob.Exists())
				{
					throw new FileNotFoundException("The item '" + relativePath + "' could not be found. Container: " + _jsonContainer.Name + " " + _jsonContainer.Uri);
				}
				string json = blob.DownloadText();
				var item = JsonConvert.DeserializeObject<TEntity>(json);
				return item;
			}
		}

		public IEnumerable<TEntity> All<TEntity>() where TEntity : class, new()
		{
			var list = new List<TEntity>();
			CloudBlobDirectory directory = null;
			if (typeof(TEntity).Equals(typeof(Image)))
			{
				directory = _imagesContainer.GetDirectoryReference(typeof(TEntity).Name.ToLower());
			}
			else
			{
				directory = _jsonContainer.GetDirectoryReference(typeof(TEntity).Name.ToLower());
			}
			var blobs = directory.ListBlobs();

			foreach (var blob in blobs)
			{
				string relativePath = string.Format("{0}{1}", blob.Parent.Prefix, Uri.UnescapeDataString(blob.Uri.Segments.LastOrDefault()));
				ICloudBlob b = null;
				if (typeof(TEntity).Equals(typeof(Image)))
				{
					b = _imagesContainer.GetBlockBlobReference(relativePath);
					var image = new Image();
					image.Url = blob.Uri.AbsoluteUri;
					var item = image as TEntity;
					list.Add(item);
				}
				else
				{
					b = _jsonContainer.GetBlockBlobReference(relativePath);
					string json = b.DownloadText();
					var entity = JsonConvert.DeserializeObject<TEntity>(json);
					list.Add(entity);
				}
			}

			return list;
		}

		public bool Exists<TEntity>(object key) where TEntity : class, new()
		{
			string relativePath = GetItemPath<TEntity>(key.ToString());
			ICloudBlob blob = null;
			if (typeof(TEntity).Equals(typeof(Image)))
			{
				blob = _imagesContainer.GetBlockBlobReference(relativePath);
			}
			else
			{
				blob = _jsonContainer.GetBlockBlobReference(relativePath);
			}
			return blob.Exists();
		}

		public void Save<TEntity>(TEntity item) where TEntity : class, new()
		{
			var key = _keys.GetKeyValue(item).ToString();
			string relativePath = GetItemPath<TEntity>(key);
			ICloudBlob blob = null;
			if (typeof(TEntity).Equals(typeof(Image)))
			{
				blob = _imagesContainer.GetBlockBlobReference(relativePath);
				var image = item as Image;
				blob.UploadFromStream(image.File.InputStream);
			}
			else
			{
				blob = _jsonContainer.GetBlockBlobReference(relativePath);
				var json = JsonConvert.SerializeObject(item, Formatting.Indented);
				blob.UploadText(json);
			}
		}

		public void Delete<TEntity>(object key) where TEntity : class, new()
		{
			string relativePath = GetItemPath<TEntity>(key.ToString());
			ICloudBlob blob = null;
			if (typeof(TEntity).Equals(typeof(Image)))
			{
				blob = _imagesContainer.GetBlockBlobReference(relativePath);
			}
			else
			{
				blob = _jsonContainer.GetBlockBlobReference(relativePath);
			}
			blob.Delete();
		}
	}
}