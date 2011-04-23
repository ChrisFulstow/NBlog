using System.Web.Mvc;
using NBlog.Web.Application;
using NBlog.Web.Application.Service;
using SquishIt.Framework;
using SquishIt.Framework.Css.Compressors;
using SquishIt.Framework.JavaScript.Minifiers;

namespace NBlog.Web.Controllers
{
    public partial class ScriptController : LayoutController
    {
        public ScriptController(IServices services) : base(services) { }

        [HttpGet]
        public ActionResult Css()
        {
            const string cacheKey = "nblog-css";

            Bundle.Css()
                .Add("~/resources/css/style.css")
                .Add(Services.Theme.Current.Css("style"))
                .Add("~/resources/scripts/jqueryopenidplugin/openid.css")
                .Add("~/resources/scripts/wmd/wmd.css")
                .Add("~/resources/scripts/prettify/prettify.css")
                .ForceRelease()
                .WithCompressor(CssCompressors.YuiCompressor)
                .AsCached(cacheKey, "");

            var css = Bundle.Css().RenderCached(cacheKey);

            return Content(css, "text/css");
        }

        [HttpGet]
        public ActionResult JavaScript()
        {
            const string cacheKey = "nblog-js";

            Bundle.JavaScript()
                .Add("~/resources/scripts/wmd/jquery.wmd.js").WithMinifier(JavaScriptMinifiers.NullMinifier)
                .Add("~/resources/scripts/plugins.js").WithMinifier(JavaScriptMinifiers.NullMinifier)
                .Add("~/resources/scripts/fancybox/jquery.mousewheel-3.0.4.pack.js").WithMinifier(JavaScriptMinifiers.NullMinifier)
                .Add("~/resources/scripts/fancybox/jquery.fancybox-1.3.2.js").WithMinifier(JavaScriptMinifiers.NullMinifier)
                .Add("~/resources/scripts/jQueryOpenIdPlugin/jquery.openid.js").WithMinifier(JavaScriptMinifiers.NullMinifier)
                .Add("~/resources/scripts/jquery.validate.js").WithMinifier(JavaScriptMinifiers.NullMinifier)
                .Add("~/resources/scripts/jquery.validate.unobtrusive.js").WithMinifier(JavaScriptMinifiers.NullMinifier)
                .Add("~/resources/scripts/prettify/prettify.js").WithMinifier(JavaScriptMinifiers.NullMinifier)
                .Add("~/resources/scripts/jquery.watermark.js").WithMinifier(JavaScriptMinifiers.NullMinifier)
                .ForceRelease().WithMinifier(JavaScriptMinifiers.Ms).AsCached(cacheKey, "");

            var js = Bundle.JavaScript().RenderCached(cacheKey);

            return Content(js, "text/javascript");
        }
    }
}