using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using NBlog.Web.Application;
using NBlog.Web.Application.Infrastructure;
using NBlog.Web.Application.Service;

namespace NBlog.Web.Controllers
{
    public partial class ContactController : LayoutController
    {
        public ContactController(IServices services) : base(services) { }

        [HttpGet]
        public ActionResult Index()
        {
            return View(new IndexModel());
        }

        [HttpPost]
        public ActionResult Index(IndexModel model)
        {
            if (!ModelState.IsValid)
                return View();

            var from = new MailAddress(model.SenderEmail, model.SenderName);
            var to = new MailAddress(Services.Config.Current.ContactForm.RecipientEmail, Services.Config.Current.ContactForm.RecipientName);
            var mailMessage = new MailMessage(from, to)
            {
                Subject = Services.Config.Current.ContactForm.Subject,
                Body = model.Message,
                IsBodyHtml = false
            };

            Services.Message.SendEmail(mailMessage);

            return RedirectToAction("Confirm");
        }

        [HttpGet]
        public ActionResult Confirm()
        {
            return View();
        }
    }
}
