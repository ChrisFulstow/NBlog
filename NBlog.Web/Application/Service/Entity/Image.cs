using System.IO;

namespace NBlog.Web.Application.Service.Entity
{
	public class Image
	{
		public string FileName { get; set; }
		public string Url { get; set; }
		public Stream StreamToUpload { get; set; }
	}
}