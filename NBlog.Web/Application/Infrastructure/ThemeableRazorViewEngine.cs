using System.Web.Mvc;
using NBlog.Web.Application.Service;

namespace NBlog.Web.Application.Infrastructure
{
    public class ThemeableRazorViewEngine : RazorViewEngine
    {
        private readonly IThemeService _themeService;

        public ThemeableRazorViewEngine(IThemeService themeService)
        {
            _themeService = themeService;

            base.ViewLocationFormats = new[]
            {
                _themeService.Current.BasePath + "/Views/{1}/{0}.cshtml",
                _themeService.Current.BasePath + "/Views/Shared/{0}.cshtml",
                "~/Themes/Default/Views/{1}/{0}.cshtml"                
            };

            base.PartialViewLocationFormats = new string[] {
                _themeService.Current.BasePath + "/Views/{1}/{0}.cshtml",
                _themeService.Current.BasePath + "/Views/Shared/{0}.cshtml",
                "~/Themes/Default/Views/Shared/{0}.cshtml"
            };
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {           
            // bypass the view cache, the view will change depending on the current theme
            const bool useViewCache = false;

            return base.FindView(controllerContext, viewName, masterName, useViewCache);
        }
    }
}