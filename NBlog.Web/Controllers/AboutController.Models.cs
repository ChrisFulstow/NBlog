using NBlog.Web.Application.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace NBlog.Web.Controllers
{
	public partial class AboutController
	{
		public class ShowModel : LayoutModel
		{
			public string Title { get; set; }
			public string Name { get; set; }
			public string Content { get; set; }
			public string ImageUrl { get; set; }
		}

		public class EditModel : LayoutModel
		{
			[AllowHtml]
			[Required(ErrorMessage = "Please enter the title of your about page.")]
			public string Title { get; set; }

			[AllowHtml]
			[Required(ErrorMessage = "Please enter your name.")]
			public string Name { get; set; }

			[AllowHtml]
			[Required(ErrorMessage = "Please enter some content.")]
			public string Content { get; set; }

			[AllowHtml]
			public HttpPostedFileBase Image { get; set; }
		}

		public class DeleteModel : LayoutModel
		{
			public string Title { get; set; }
			public string ImageUrl { get; set; }
		}
	}
}