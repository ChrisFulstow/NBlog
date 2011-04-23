using NBlog.Web.Application.Service.Entity;

namespace NBlog.Web.Application.Service
{
    public interface IThemeService
    {
        Theme Current { get; }
    }
}