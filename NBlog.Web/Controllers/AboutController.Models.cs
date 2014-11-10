using NBlog.Web.Application.Infrastructure;

namespace NBlog.Web.Controllers
{
	public partial class AboutController
	{
		public class AboutModel : LayoutModel
		{
			public string Name { get; set; }
			public string Content { get; set; }
		}
	}
}