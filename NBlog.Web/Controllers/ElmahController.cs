using NBlog.Web.Application.Infrastructure;
using System.Web.Mvc;

namespace NBlog.Web.Controllers
{
	[AdminOnly]
	public class ElmahController : Controller
	{
		public ActionResult Index(string type)
		{
			return new ElmahResult(type);
		}
	}
}