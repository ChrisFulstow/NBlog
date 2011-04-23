using NBlog.Web.Application.Service.Entity;

namespace NBlog.Web.Application.Service
{
    public interface IUserService
    {
        User Current { get; }
    }
}