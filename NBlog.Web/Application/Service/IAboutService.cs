using NBlog.Web.Application.Service.Entity;
using System.Collections.Generic;

namespace NBlog.Web.Application.Service
{
	public interface IAboutService
	{
		List<About> GetAll();
	}
}
