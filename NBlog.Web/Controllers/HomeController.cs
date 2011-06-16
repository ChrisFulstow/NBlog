using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NBlog.Web.Application;
using NBlog.Web.Application.Infrastructure;
using NBlog.Web.Application.Service;

namespace NBlog.Web.Controllers
{
    public partial class HomeController : LayoutController
    {
        public HomeController(IServices services) : base(services) { }

        [HttpGet]
        public ViewResult Index()
        {
            var entries = Services.Entry.GetList();

            var model = new IndexModel
            {
                Entries = entries
                    .OrderByDescending(e => e.DateCreated)
                    .Select(e => new EntrySummaryModel
                    {
                        Key = e.Slug,
                        Title = e.Title,
                        Date = e.DateCreated.ToDateString(),
                        PrettyDate = e.DateCreated.ToPrettyDate(),
                        IsPublished = e.IsPublished ?? true
                    })
            };

            return View(model);
        }

        [HttpGet]
        public ViewResult Throw()
        {
            throw new NotImplementedException("Example exception for testing error handling.");
        }

        [HttpGet]
        public ActionResult WhoAmI()
        {
            return Content(User.Identity.Name.AsNullIfEmpty() ?? "Not logged in");
        }

        [HttpGet]
        public ActionResult WhatTimeIsIt()
        {
            return Content(DateTime.Now.ToString());
        }
    }
}
