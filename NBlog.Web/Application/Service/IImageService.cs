using NBlog.Web.Application.Service.Entity;

namespace NBlog.Web.Application.Service
{
	public interface IImageService
	{
		void Save(Image stream);
		Image GetByFileName(string fileName);
		void Delete(string fileName);
	}
}
