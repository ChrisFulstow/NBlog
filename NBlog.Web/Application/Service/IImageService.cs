using NBlog.Web.Application.Service.Entity;
using System.Collections.Generic;

namespace NBlog.Web.Application.Service
{
	public interface IImageService
	{
		void Save(Image entry);
		Image GetByFileName(string fileName);
		List<Image> GetList();
		void Delete(string fileName);
		bool Exists(string fileName);
	}
}
