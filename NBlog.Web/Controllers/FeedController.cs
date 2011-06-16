using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Web.Mvc;
using NBlog.Web.Application;
using NBlog.Web.Application.Infrastructure;
using NBlog.Web.Application.Service;

namespace NBlog.Web.Controllers
{
    public class FeedController : Controller
    {
        private readonly IServices _services;

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
                    markdown.Transform(e.Markdown),
                    new Uri(baseUri, Url.Action("Show", "Entry", new { id = e.Slug }, null))));

            var feed = new SyndicationFeed(
                title: _services.Config.Current.Heading,
                description: _services.Config.Current.Tagline,
                feedAlternateLink: new Uri(baseUri, Url.Action("Index", "Feed")),
                items: entries);

            return new RssResult(feed);
        }
    }
}
