using NBlog.Web.Application.Service.Entity;
using NBlog.Web.Application.Storage;
using System.Collections.Generic;
using System.Linq;

namespace NBlog.Web.Application.Service.Internal
{
	public class AboutService : IAboutService
	{
		private readonly IRepository _repository;

		public AboutService(IRepository repository)
		{
			_repository = repository;
		}

		public List<About> GetAll()
		{
			return _repository.All<About>().ToList();
		}

		public void Save(About about)
		{
			_repository.Save<About>(about);
		}

		public About GetByTitle(string title)
		{
			return _repository.Single<About>(title);
		}

		public bool Exists(string title)
		{
			return _repository.Exists<About>(title);
		}

		public void Delete(string title)
		{
			_repository.Delete<About>(title);
		}
	}
}