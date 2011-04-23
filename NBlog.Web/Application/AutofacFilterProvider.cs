using System.Collections.Generic;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Web;

namespace NBlog.Web.Application
{
    public class AutofacFilterProvider : FilterAttributeFilterProvider
    {
        private readonly IContainerProvider _containerProvider;

        public AutofacFilterProvider(IContainerProvider containerProvider)
        {
            _containerProvider = containerProvider;
        }

        public override IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var filters = base.GetFilters(controllerContext, actionDescriptor);

            foreach (var filter in filters)
            {
                _containerProvider.RequestLifetime.InjectProperties(filter.Instance);
            }

            return filters;
        }
    }
}