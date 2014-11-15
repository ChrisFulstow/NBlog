using NBlog.Web.Application.Service.Entity;
using NBlog.Web.Application.Storage;

namespace NBlog.Web.Application.Service.Internal
{
	public class ImageService : IImageService
	{
		private readonly IRepository _repository;

		public ImageService(IRepository repository)
		{
			_repository = repository;
		}

		public void Save(Image stream)
		{
			_repository.Save<Image>(stream);
		}

		public Image GetByFileName(string fileName)
		{
			return _repository.Single<Image>(fileName);
		}

		public void Delete(string fileName)
		{
			_repository.Delete<Image>(fileName);
		}
	}
}