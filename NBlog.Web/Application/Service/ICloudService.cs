namespace NBlog.Web.Application.Service
{
    public interface ICloudService
    {
        string ArchiveFolder(string folderPath);

        void SetUp();
    }
}