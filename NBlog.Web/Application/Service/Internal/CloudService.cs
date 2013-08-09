using DropNet;
using DropNet.Models;
using Ionic.Zip;
using NBlog.Web.Application.Infrastructure;
using NBlog.Web.Application.Storage;
using System;
using System.IO;

namespace NBlog.Web.Application.Service.Internal
{
    public class CloudService : ICloudService
    {
        private const string ArchiveFolderPath = "/NBlog";

        private readonly IConfigService _configService;
        private readonly DropNetClient _dropBoxClient;

        public CloudService(IConfigService configService)
        {
            _configService = configService;
            var config = _configService.Current;

            _dropBoxClient = new DropNetClient(
                apiKey: config.Cloud.ConsumerKey,
                appSecret: config.Cloud.ConsumerSecret,
                userToken: config.Cloud.UserToken,
                userSecret: config.Cloud.UserSecret);
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

        /// <summary>
        /// Use a breakpoint to set-up a connection to your DropBox app and follow the described
        /// steps
        /// </summary>
        public void SetUp()
        {
            _dropBoxClient.GetToken();

            // copy URL into the browser and log into dropbox
            var url = _dropBoxClient.BuildAuthorizeUrl();

            // save token information into the config
            var token = _dropBoxClient.GetAccessToken();

            _dropBoxClient.UserLogin = token;
        }

        private static bool FolderExists(DropNet.Models.MetaData backupFolder)
        {
            return backupFolder != null && !backupFolder.Is_Deleted && backupFolder.Is_Dir;
        }

        private void Save(string filename, MemoryStream memoryStream)
        {
            var backupFolder = _dropBoxClient.GetMetaData(ArchiveFolderPath);
            if (!FolderExists(backupFolder)) { throw new Exception("Cloud folder not found: " + ArchiveFolderPath); }

            memoryStream.Position = 0;
            _dropBoxClient.UploadFile(ArchiveFolderPath, filename, memoryStream);
        }
    }
}