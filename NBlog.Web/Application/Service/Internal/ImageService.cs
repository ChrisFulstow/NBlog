using NBlog.Web.Application.Service.Entity;
using NBlog.Web.Application.Storage;
using System.Collections.Generic;
using System.Linq;

namespace NBlog.Web.Application.Service.Internal
{
	public class ImageService : IImageService
	{
		private readonly IRepository _repository;

		public ImageService(IRepository repository)
		{
			_repository = repository;
		}

		public void Save(Image image)
		{
			_repository.Save<Image>(image);
		}

		public Image GetByFileName(string fileName)
		{
			return _repository.Single<Image>(fileName);
		}

		public List<Image> GetList()
		{
			return _repository.All<Image>().ToList();
		}

		public void Delete(string fileName)
		{
			_repository.Delete<Image>(fileName);
		}

		public bool Exists(string fileName)
		{
			return _repository.Exists<Image>(fileName);
		}
	}
}