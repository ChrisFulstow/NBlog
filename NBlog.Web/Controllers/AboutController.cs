using NBlog.Web.Application.Infrastructure;
using NBlog.Web.Application.Service;
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
			if (Services.Entry.Exists("about"))
			{
				var about = Services.Entry.GetBySlug("about");
				model.Name = about.Author;
				model.Content = about.Markdown;
			}

			return View(model);
		}
	}
}