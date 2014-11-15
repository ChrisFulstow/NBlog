using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
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
		private readonly CloudBlobClient _blobClient;

		public AzureBlobRepository(RepositoryKeys keys)
		{
			_keys = keys;
			_blobClient = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureBlob"].ConnectionString).CreateCloudBlobClient();
		}

		private string GetItemPath<TEntity>(string key)
		{
			string pathFormat = "{0}/{1}";
			if (!typeof(TEntity).Equals(typeof(Image)))
			{
				pathFormat = string.Format("{0}.json", pathFormat);
			}
			return string.Format(pathFormat, typeof(TEntity).Name.ToLower(), key);
		}

		private CloudBlobContainer GetBlobContainer<TEntity>()
		{
			CloudBlobContainer blobContainer = null;
			if (!typeof(TEntity).Equals(typeof(Image)))
			{
				blobContainer = _blobClient.GetContainerReference(ConfigurationManager.AppSettings["JsonBlobContainerName"]);
				blobContainer.CreateIfNotExists(BlobContainerPublicAccessType.Off);
			}
			else
			{
				blobContainer = _blobClient.GetContainerReference(ConfigurationManager.AppSettings["ImagesBlobContainerName"]);
				blobContainer.CreateIfNotExists(BlobContainerPublicAccessType.Blob);
			}
			return blobContainer;
		}

		public TEntity Single<TEntity>(object key) where TEntity : class, new()
		{
			string relativePath = GetItemPath<TEntity>(key.ToString());
			var blobContainer = GetBlobContainer<TEntity>();
			ICloudBlob blob = blobContainer.GetBlockBlobReference(relativePath);
			if (!blob.Exists())
			{
				throw new FileNotFoundException("The item '" + relativePath + "' could not be found. Container: " + blobContainer.Name + " " + blobContainer.Uri);
			}
			TEntity item = null;
			if (!typeof(TEntity).Equals(typeof(Image)))
			{
				string json = blob.DownloadText();
				item = JsonConvert.DeserializeObject<TEntity>(json);
			}
			else
			{
				var image = new Image() { Uri = blob.Uri.AbsoluteUri };
				item = image as TEntity;
			}
			return item;
		}

		public IEnumerable<TEntity> All<TEntity>() where TEntity : class, new()
		{
			var list = new List<TEntity>();
			var blobContainer = GetBlobContainer<TEntity>();
			CloudBlobDirectory directory = blobContainer.GetDirectoryReference(typeof(TEntity).Name.ToLower());
			var blobs = directory.ListBlobs();

			foreach (var blob in blobs)
			{
				string relativePath = string.Format("{0}{1}", blob.Parent.Prefix, Uri.UnescapeDataString(blob.Uri.Segments.LastOrDefault()));
				ICloudBlob b = blobContainer.GetBlockBlobReference(relativePath);
				string json = b.DownloadText();
				var entity = JsonConvert.DeserializeObject<TEntity>(json);
				list.Add(entity);
			}

			return list;
		}

		public bool Exists<TEntity>(object key) where TEntity : class, new()
		{
			string relativePath = GetItemPath<TEntity>(key.ToString());
			ICloudBlob blob = GetBlobContainer<TEntity>().GetBlockBlobReference(relativePath);
			return blob.Exists();
		}

		public void Save<TEntity>(TEntity item) where TEntity : class, new()
		{
			var key = _keys.GetKeyValue(item).ToString();
			string relativePath = GetItemPath<TEntity>(key);
			var blobContainer = GetBlobContainer<TEntity>();
			ICloudBlob blob = blobContainer.GetBlockBlobReference(relativePath);
			if (!typeof(TEntity).Equals(typeof(Image)))
			{
				var json = JsonConvert.SerializeObject(item, Formatting.Indented);
				blob.UploadText(json);
			}
			else
			{
				var image = item as Image;
				// Ensure position is reset so upload occurs successfully
				image.StreamToUpload.Seek(0, SeekOrigin.Begin);
				blob.UploadFromStream(image.StreamToUpload);
			}
		}

		public void Delete<TEntity>(object key) where TEntity : class, new()
		{
			string relativePath = GetItemPath<TEntity>(key.ToString());
			ICloudBlob blob = GetBlobContainer<TEntity>().GetBlockBlobReference(relativePath);
			blob.Delete();
		}
	}
}