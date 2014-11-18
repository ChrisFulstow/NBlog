using System;
using System.Configuration;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.Routing;

namespace NBlog.Web
{
	public class Global : HttpApplication
	{
		private void Application_Start(object sender, EventArgs e)
		{
			// Code that runs on application startup
			// Need this line so injection on controllers that use other assemblies can happen correctly
			BuildManager.GetReferencedAssemblies();
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			ContainerConfig.SetUpContainer();
			// Override the Instrumentation Key from ApplicationInsights.config, so we can set it from the app settings in the azure configuration dashboard
			Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["ApplicationInsightsInstrumentationKey"];
		}

		private void Application_End(object sender, EventArgs e)
		{
			// Code that runs on application shutdown
		}

		private void Application_Error(object sender, EventArgs e)
		{
			if (!HttpContext.Current.IsCustomErrorEnabled)
				return;

			var exception = Server.GetLastError();
			var httpException = new HttpException(null, exception);

			var routeData = new RouteData();
			routeData.Values.Add("controller", "Error");
			routeData.Values.Add("action", "Index");
			routeData.Values.Add("httpException", httpException);

			Server.ClearError();

			var errorController = ControllerBuilder.Current.GetControllerFactory().CreateController(
				new RequestContext(new HttpContextWrapper(Context), routeData), "Error");

			errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
		}
	}
}
