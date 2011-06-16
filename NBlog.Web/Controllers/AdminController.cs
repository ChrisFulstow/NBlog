using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NBlog.Web.Application;
using NBlog.Web.Application.Infrastructure;
using NBlog.Web.Application.Service;
using NBlog.Web.Application.Storage;
using NBlog.Web.Application.Storage.Json;

namespace NBlog.Web.Controllers
{
    public partial class AdminController : LayoutController
    {
        private readonly IRepository _repository;

        public AdminController(IServices services, IRepository repositoryToBackup)
            : base(services)
        {
            _repository = repositoryToBackup;
        }

        [AdminOnly]
        [HttpGet]
        public ActionResult Backup()
        {
            var jsonRepository = _repository as JsonRepository;
            
            if (jsonRepository == null)
                throw new Exception("Backup currently supports only JsonRepository");

            var backupFilename = Services.Cloud.ArchiveFolder(jsonRepository.DataPath);

            return Content("Backup complete: " + backupFilename);
        }
    }

}