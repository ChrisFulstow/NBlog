using Autofac;
using Autofac.Integration.Mvc;
using Quartz;
using Quartz.Simpl;
using Quartz.Spi;

namespace NBlog.Web.Application.Infrastructure
{
    public class AutofacJobFactory : SimpleJobFactory
    {
        private readonly ILifetimeScopeProvider _containerProvider;

        public AutofacJobFactory(ILifetimeScopeProvider containerProvider)
        {
            _containerProvider = containerProvider;
        }

        public override IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var job = base.NewJob(bundle, scheduler);
            _containerProvider.ApplicationContainer.InjectProperties(job);
            return job;
        }
    }
}