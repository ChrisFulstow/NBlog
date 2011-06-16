using System;
using System.IO;
using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.DropBox;
using Ionic.Zip;
using NBlog.Web.Application.Infrastructure;
using NBlog.Web.Application.Storage;

namespace NBlog.Web.Application.Service.Internal
{
    public class CloudService : ICloudService
    {
        private const string ArchiveFolderPath = "/NBlog";
        private readonly ICloudStorageConfiguration _cloudStorageConfiguration;
        private readonly ICloudeStorageCredentials _cloudeStorageCredentials;
        private readonly IConfigService _configService;

        public CloudService(IConfigService configService)
        {
            _configService = configService;
            var config = _configService.Current;

            // currently using DropBox, but this could be made configurable

            _cloudStorageConfiguration = DropBoxConfiguration.GetStandardConfiguration();

            _cloudeStorageCredentials = new DropBoxCredentials
            {
                ConsumerKey = config.Cloud.ConsumerKey,
                ConsumerSecret = config.Cloud.ConsumerSecret,
                UserName = config.Cloud.UserName,
                Password = config.Cloud.Password
            };
        }

        public string ArchiveFolder(string folderPath)
        {
            var backupName = Path.GetFileName(folderPath).ToUrlSlug();
            var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var archiveFilename = string.Format("{0}-{1}.zip", backupName, timestamp);

            using (var zip = new ZipFile())
            using (var zipMemoryStream = new MemoryStream())
            {
                zip.AddDirectory(folderPath);
                zip.Save(zipMemoryStream);
                Save(archiveFilename, zipMemoryStream);
            }

            return archiveFilename;
        }

        private void Save(string filename, MemoryStream memoryStream)
        {
            var storage = new CloudStorage();
            storage.Open(_cloudStorageConfiguration, _cloudeStorageCredentials);

            var backupFolder = storage.GetFolder(ArchiveFolderPath);
            if (backupFolder == null) { throw new Exception("Cloud folder not found: " + ArchiveFolderPath); }

            var cloudFile = storage.CreateFile(backupFolder, filename);
            using (var cloudStream = cloudFile.GetContentStream(FileAccess.Write))
            {
                cloudStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
            }

            if (storage.IsOpened) { storage.Close(); }
        }

    }
}