namespace NBlog.Web.Application.Storage.Azure
{
	using Microsoft.WindowsAzure.Storage.Blob;
	using System.IO;

	public static class AzureExtensions
	{
		public static void UploadText(this ICloudBlob blob, string text)
		{
			byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);
			blob.UploadFromByteArray(buffer, 0, buffer.Length);
		}

		public static string DownloadText(this ICloudBlob blob)
		{
			string text;
			using (var memoryStream = new MemoryStream())
			{
				blob.DownloadToStream(memoryStream);
				text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
			}
			return text;
		}

		//public static bool Exists(this ICloudBlob blob)
		//{
		//    try
		//    {
		//        blob.FetchAttributes();
		//        return true;
		//    }
		//    catch (StorageException e)
		//    {
		//        if (e.RequestInformation.ExtendedErrorInformation != null &&
		//            e.RequestInformation.ExtendedErrorInformation.ErrorCode == StorageErrorCodeStrings.ResourceNotFound)
		//        {
		//            return false;
		//        }
		//        else
		//        {
		//            throw;
		//        }
		//    }
		//}

		//public static bool Exists(this CloudBlobContainer container)
		//{
		//    try
		//    {
		//        container.FetchAttributes();
		//        return true;
		//    }
		//    catch (StorageException e)
		//    {
		//        if (e.RequestInformation.ExtendedErrorInformation != null &&
		//            e.RequestInformation.ExtendedErrorInformation.ErrorCode == StorageErrorCodeStrings.ResourceNotFound)
		//        {
		//            return false;
		//        }
		//        else
		//        {
		//            throw;
		//        }
		//    }
		//}
	}
}