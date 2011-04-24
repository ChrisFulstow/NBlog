using System;
using NBlog.Web.Application.Service;
using NBlog.Web.Application.Storage;
using NBlog.Web.Application.Storage.Json;
using Quartz;

namespace NBlog.Web.Application.Job
{
    public class Backup : IJob
    {
        public ICloudService Cloud { get; set; }
        public IRepository Repsitory { get; set; }

        public void Execute(JobExecutionContext context)
        {
            var jsonRepository = Repsitory as JsonRepository;

            if (jsonRepository == null)
                throw new Exception("Backup currently supports only JsonRepository");

            Cloud.ArchiveFolder(jsonRepository.DataPath);
        }
    }
}