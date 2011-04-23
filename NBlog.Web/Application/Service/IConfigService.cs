using NBlog.Web.Application.Service.Entity;

namespace NBlog.Web.Application.Service
{
    public interface IConfigService
    {
        Config Current { get; }
    }
}