using System.Web;

namespace NBlog.Web.Application.Service.Entity
{
	public class Image
	{
		public HttpPostedFileBase File { get; set; }
		public string Url { get; set; }
	}
}