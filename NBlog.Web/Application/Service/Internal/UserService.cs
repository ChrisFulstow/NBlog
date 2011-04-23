using System;
using System.Linq;
using System.Web;
using System.Web.Security;
using NBlog.Web.Application.Service.Entity;

namespace NBlog.Web.Application.Service.Internal
{
    public class UserService : IUserService
    {
        private readonly IConfigService _configService;

        public UserService(IConfigService configService)
        {
            _configService = configService;

            var identity = HttpContext.Current.User.Identity;
            var formsIdentity = identity as FormsIdentity;
            var friendlyName = formsIdentity != null ? formsIdentity.Ticket.UserData : identity.Name;
            if (string.IsNullOrEmpty(friendlyName)) { friendlyName = identity.Name; }

            var isAdmin =
                identity.IsAuthenticated
                && _configService.Current.Admins != null
                && _configService.Current.Admins.Contains(identity.Name, StringComparer.InvariantCultureIgnoreCase);

            var user = new User
            {
                FriendlyName = friendlyName, 
                IsAuthenticated = identity.IsAuthenticated,
                IsAdmin = isAdmin
            };

            Current = user;
        }

        public User Current { get; private set; }
    }
}