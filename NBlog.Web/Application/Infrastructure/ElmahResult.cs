using System;
using System.Web;
using System.Web.Mvc;

namespace NBlog.Web.Application.Infrastructure
{
	/// <summary>
	/// Code supplied by Alexander Beletsky: http://beletsky.net/2011/03/integrating-elmah-to-aspnet-mvc-in.html
	/// </summary>
	public class ElmahResult : ActionResult
	{
		private string _resouceType;

		public ElmahResult(string resouceType)
		{
			_resouceType = resouceType;
		}

		public override void ExecuteResult(ControllerContext context)
		{
			var factory = new Elmah.ErrorLogPageFactory();

			if (!string.IsNullOrEmpty(_resouceType))
			{
				var pathInfo = "." + _resouceType;
				HttpContext.Current.RewritePath(PathForStylesheet(), pathInfo, HttpContext.Current.Request.QueryString.ToString());
			}

			var httpHandler = factory.GetHandler(HttpContext.Current, null, null, null);
			httpHandler.ProcessRequest(HttpContext.Current);
		}

		private string PathForStylesheet()
		{
			return _resouceType != "stylesheet" ? HttpContext.Current.Request.Path.Replace(String.Format("/{0}", _resouceType), string.Empty) : HttpContext.Current.Request.Path;
		}
	}
}