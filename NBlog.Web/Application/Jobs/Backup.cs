using NBlog.Web.Application.Service;
using NBlog.Web.Application.Storage;
using NBlog.Web.Application.Storage.Json;
using Quartz;
using System;

namespace NBlog.Web.Application.Jobs
{
    public class Backup : IJob
    {
        public ICloudService Cloud { get; set; }

        public IRepository Repsitory { get; set; }

        public void Execute(IJobExecutionContext context)
        {
            var jsonRepository = Repsitory as JsonRepository;

            if (jsonRepository == null)
                throw new Exception("Backup currently supports only JsonRepository");

            Cloud.ArchiveFolder(jsonRepository.DataPath);
        }
    }
}