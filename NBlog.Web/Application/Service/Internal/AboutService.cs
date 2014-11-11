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
	}
}