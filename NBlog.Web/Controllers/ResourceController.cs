using NBlog.Web.Application;
using NBlog.Web.Application.Infrastructure;
using NBlog.Web.Application.Service;
using SquishIt.Framework;
using SquishIt.Framework.Minifiers.CSS;
using System.Web.Mvc;
using JavaScriptMinifiers = SquishIt.Framework.Minifiers.JavaScript;

namespace NBlog.Web.Controllers
{
    public class ResourceController : LayoutController
    {
        public ResourceController(IServices services)
            : base(services)
        {
        }

        [HttpGet]
        public ActionResult Favicon()
        {
            return File(Services.Theme.Current.BasePath + "/favicon.ico", "image/x-icon");
        }

        [HttpGet]
        public ActionResult Css()
        {
            var cacheKey = Services.Theme.Current.Name + "-css";

            Bundle.Css()
                .Add(Services.Theme.Current.Css("style"))
                .Add("~/scripts/jqueryopenidplugin/openid.css")
                .Add("~/scripts/wmd/wmd.css")
                .Add("~/scripts/prettify/prettify.css")
                .ForceRelease()
                .WithMinifier(new MsMinifier())
                .AsCached(cacheKey, "");

            var css = Bundle.Css().RenderCached(cacheKey);

            return Content(css, "text/css");
        }

        [HttpGet]
        public ActionResult JavaScript()
        {
            const string cacheKey = "nblog-js";

            Bundle.JavaScript()
                .Add("~/scripts/wmd/jquery.wmd.js").WithMinifier(new JavaScriptMinifiers.NullMinifier())
                .Add("~/scripts/plugins.js").WithMinifier(new JavaScriptMinifiers.NullMinifier())
                .Add("~/scripts/fancybox/jquery.mousewheel-3.0.4.pack.js").WithMinifier(new JavaScriptMinifiers.NullMinifier())
                .Add("~/scripts/fancybox/jquery.fancybox-1.3.2.js").WithMinifier(new JavaScriptMinifiers.NullMinifier())
                .Add("~/scripts/jQueryOpenIdPlugin/jquery.openid.js").WithMinifier(new JavaScriptMinifiers.NullMinifier())
                .Add("~/scripts/jquery.validate.js").WithMinifier(new JavaScriptMinifiers.NullMinifier())
                .Add("~/scripts/jquery.validate.unobtrusive.js").WithMinifier(new JavaScriptMinifiers.NullMinifier())
                .Add("~/scripts/prettify/prettify.js").WithMinifier(new JavaScriptMinifiers.NullMinifier())
                .Add("~/scripts/jquery.watermark.js").WithMinifier(new JavaScriptMinifiers.NullMinifier())
                .ForceRelease().WithMinifier(new JavaScriptMinifiers.MsMinifier()).AsCached(cacheKey, "");

            var js = Bundle.JavaScript().RenderCached(cacheKey);

            return Content(js, "text/javascript");
        }
    }
}