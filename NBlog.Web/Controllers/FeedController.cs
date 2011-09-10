using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using NBlog.Web.Application.Infrastructure;
using NBlog.Web.Application.Service;

namespace NBlog.Web.Controllers
{
    public class FeedController : Controller
    {
        private readonly IServices _services;
        private static readonly Regex _linkRegex = new Regex(@"(?<=\[[^\]]*\]\()(?<url>[^\)]*)(?=\))");

        public FeedController(IServices services)
        {
            _services = services;
        }

        public ActionResult Index()
        {
            var markdown = new MarkdownSharp.Markdown();
            var baseUri = new Uri(Request.Url.GetLeftPart(UriPartial.Authority));
            
            var entries =
                _services.Entry.GetList()
                .Where(e => e.IsPublished ?? true)
                .OrderByDescending(e => e.DateCreated)
                .Take(10)
                .Select(e => new SyndicationItem(
                    e.Title,
                    markdown.Transform(UrlsToAbsolute(e.Markdown)),
                    new Uri(baseUri, Url.Action("Show", "Entry", new { id = e.Slug }, null))));

            var feed = new SyndicationFeed(
                title: _services.Config.Current.Heading,
                description: _services.Config.Current.Tagline,
                feedAlternateLink: new Uri(baseUri, Url.Action("Index", "Feed")),
                items: entries);

            return new RssResult(feed);
        }

        private string UrlsToAbsolute(string markdown)
        {
            return _linkRegex.Replace(markdown, ToAbsoluteUrl);
        }

        private string ToAbsoluteUrl(Match match)
        {
            var url = match.Groups["url"].Value;
            if (!(Uri.IsWellFormedUriString(url, UriKind.Absolute)))
            {
                return ToPublicUrl(Request.RequestContext.HttpContext, new Uri(url, UriKind.Relative));
            }
            return url;
        }

        private static string ToPublicUrl(HttpContextBase httpContext, Uri relativeUri)
        {
            var uriBuilder = new UriBuilder
            {
                Host = httpContext.Request.Url.Host,
                Path = "/",
                Port = 80,
                Scheme = "http",
            };

            if (httpContext.Request.IsLocal)
            {
                uriBuilder.Port = httpContext.Request.Url.Port;
            }

            return new Uri(uriBuilder.Uri, relativeUri).AbsoluteUri;
        }
    }
}
