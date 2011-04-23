using Autofac;
using Autofac.Integration.Web;
using Quartz;
using Quartz.Simpl;
using Quartz.Spi;

namespace NBlog.Web.Application.Job
{
    public class AutofacJobFactory : SimpleJobFactory
    {
        private readonly IContainerProvider _containerProvider;

        public AutofacJobFactory(IContainerProvider containerProvider)
        {
            _containerProvider = containerProvider;
        }

        public override IJob NewJob(TriggerFiredBundle bundle)
        {
            var job = base.NewJob(bundle);
            _containerProvider.ApplicationContainer.InjectProperties(job);
            return job;
        }
    }
}