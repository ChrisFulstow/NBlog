using NBlog.Web.Application;
using NBlog.Web.Application.Infrastructure;
using NBlog.Web.Application.Service;
using NBlog.Web.Application.Storage;
using NBlog.Web.Application.Storage.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NBlog.Web.Controllers
{
    public partial class AdminController : LayoutController
    {
        private readonly IRepository _repository;

        public AdminController(IServices services, IRepository repository)
            : base(services)
        {
            _repository = repository;
        }

        [AdminOnly]
        [HttpGet]
        public ActionResult Backup()
        {
            var jsonRepository = GetJsonRepository();
            var backupFilename = Services.Cloud.ArchiveFolder(jsonRepository.DataPath);

            return Content("Backup complete: " + backupFilename);
        }

        /// <summary>
        /// Just to be used once to set up a connection to your dropbox-app. Set a breakpoint in
        /// CloudSerice.SetUp and follow the described steps.
        /// </summary>
        /// <returns></returns>
        [AdminOnly]
        [HttpGet]
        public ActionResult SetUp()
        {
            Services.Cloud.SetUp();
            return RedirectToAction("Index", "Home");
        }

        private JsonRepository GetJsonRepository()
        {
            var jsonRepository = _repository as JsonRepository;

            if (jsonRepository == null)
                throw new Exception("Backup currently supports only JsonRepository");

            return jsonRepository;
        }
    }
}