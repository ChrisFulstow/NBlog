using NBlog.Web.Application.Infrastructure;
using NBlog.Web.Application.Service;
using System.Linq;
using System.Web.Mvc;

namespace NBlog.Web.Controllers
{
	public partial class AboutController : LayoutController
	{
		public AboutController(IServices services) : base(services) { }

		// GET: About
		[HttpGet]
		public ActionResult Index()
		{
			var model = new AboutModel();
			var about = Services.About.GetAll().FirstOrDefault();
			if (about != null)
			{
				model.Name = about.Name;
				model.Title = about.Title;
				model.Content = about.Content;
			}

			return View(model);
		}
	}
}