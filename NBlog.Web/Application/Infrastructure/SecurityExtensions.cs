using System.Web;
using System.Web.Mvc;
using Microsoft.Security.Application;

namespace NBlog.Web.Application.Infrastructure
{
    public static class SecurityExtensions
    {
        /// <summary>
        /// Sanitises HTML fragment for protection against XSS vulnerabilities
        /// </summary>
        public static IHtmlString Safe(this HtmlHelper html, string unsafeHtml)
        {
            var safeHtml = Sanitizer.GetSafeHtmlFragment(unsafeHtml);
            return MvcHtmlString.Create(safeHtml);
        }
    }
}