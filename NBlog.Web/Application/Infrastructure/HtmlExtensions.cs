using System.Web;
using System.Web.Mvc;

namespace NBlog.Web.Application.Infrastructure
{
    public static class HtmlExtensions
    {
        public static IHtmlString Raw(this HtmlHelper htmlHelper, string html)
        {
            return MvcHtmlString.Create(html);
        }

        public static IHtmlString Concat(this HtmlHelper htmlHelper, params IHtmlString[] strings)
        {
            var concat = string.Join<IHtmlString>("", strings);
            return MvcHtmlString.Create(concat);
        }

        public static IHtmlString Blank(this HtmlHelper htmlHelper)
        {
            return MvcHtmlString.Empty;
        }
    }
}