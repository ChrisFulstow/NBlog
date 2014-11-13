using ImageResizer;
using MarkdownSharp;
using NBlog.Web.Application.Infrastructure;
using NBlog.Web.Application.Service;
using NBlog.Web.Application.Service.Entity;
using System;
using System.IO;
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
				model.ImageUrl = about.ImageUrl;
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
			about.Name = model.Name;
			about.Title = model.Title;
			about.Content = model.Content;
			// Save the image in blob storage
			if (model.Image != null)
			{
				var imageFileName = Path.GetFileName(model.Image.FileName);
				var image = Services.Image.Exists(imageFileName) ? Services.Image.GetByFileName(imageFileName) : new NBlog.Web.Application.Service.Entity.Image();
				// Scale the image before saving
				using (var scaledImageStream = new MemoryStream())
				{
					var settings = new ResizeSettings(200, 150, FitMode.None, "jpg");
					ImageBuilder.Current.Build(model.Image.InputStream, scaledImageStream, settings);
					image.FileName = imageFileName;
					image.StreamToUpload = scaledImageStream;
					Services.Image.Save(image);
				}
				// Get the url to link to the About Entity
				about.ImageUrl = Services.Image.GetByFileName(imageFileName).Url;
			}
			Services.About.Save(about);

			return RedirectToAction("Index", "About");
		}

		[AdminOnly]
		[HttpGet]
		public ActionResult Delete(string title)
		{
			return View(new DeleteModel() { Title = title });
		}

		[AdminOnly]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(DeleteModel model)
		{
			var imageUrl = Services.About.GetByTitle(model.Title).ImageUrl;
			if (!string.IsNullOrWhiteSpace(imageUrl))
			{
				Uri imageUri = new Uri(imageUrl);
				Services.Image.Delete(Path.GetFileName(imageUri.LocalPath));
			}
			Services.About.Delete(model.Title);
			return RedirectToAction("Index", "About");
		}
	}
}