﻿using MarkdownSharp;
using NBlog.Web.Application.Infrastructure;
using NBlog.Web.Application.Service;
using NBlog.Web.Application.Service.Entity;
using System.Linq;
using System.Web.Mvc;

namespace NBlog.Web.Controllers
{
	public partial class AboutController : LayoutController
	{
		public AboutController(IServices services) : base(services) { }

		// GET: About
		[HttpGet]
		public ActionResult Index()
		{
			var model = new ShowModel();
			var about = Services.About.GetAll().FirstOrDefault();
			if (about != null)
			{
				model.Name = about.Name;
				model.Title = about.Title;
				model.Content = new Markdown().Transform(about.Content);
			}

			return View(model);
		}

		[AdminOnly]
		[HttpGet]
		public ActionResult Edit(string title)
		{
			var model = new EditModel();
			var about = !string.IsNullOrEmpty(title) ? Services.About.GetByTitle(title) :
				Services.About.GetAll().FirstOrDefault();
			if (about != null)
			{
				model.Title = about.Title;
				model.Content = about.Content;
				model.Name = about.Name;
			}

			return View(model);
		}

		[AdminOnly]
		[ValidateAntiForgeryToken]
		[HttpPost]
		public ActionResult Edit(EditModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var about = Services.About.Exists(model.Title) ? Services.About.GetByTitle(model.Title) : new About();
			if (about != null)
			{
				about.Name = model.Name;
				about.Title = model.Title;
				about.Content = model.Content;
				Services.About.Save(about);
			}

			return RedirectToAction("Index", "About");
		}
	}
}