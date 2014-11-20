using Elmah;
using System.Web.Mvc;
using System.Web.Routing;

namespace NBlog.Web
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.RouteExistingFiles = false;
			routes.LowercaseUrls = true;

			// homepage
			routes.MapRoute("", "", new { controller = "Home", action = "Index" });

			// favicon
			routes.MapRoute("", "favicon.ico", new { controller = "Resource", action = "Favicon" });

			// combined scripts
			routes.MapRoute("", "min.css", new { controller = "Resource", action = "Css" });
			routes.MapRoute("", "min.js", new { controller = "Resource", action = "JavaScript" });

			// feed
			routes.MapRoute("", "feed", new { controller = "Feed", action = "Index" });

			// search
			routes.MapRoute("", "search", new { controller = "Search", action = "Index" });

			// contact
			routes.MapRoute("", "contact", new { controller = "Contact", action = "Index" });

			// about
			routes.MapRoute("", "about", new { controller = "About", action = "Index" });
			
			// Elmah
			routes.MapRoute("", "elmah/{type}", new { controller = "Elmah", action = "Index", type = UrlParameter.Optional });			

			// entry pages
			routes.MapRoute("", "{id}", new { controller = "Entry", action = "Show" });

			// general route
			routes.MapRoute("", "{controller}/{action}/{id}", new { id = UrlParameter.Optional });
		}
	}
}