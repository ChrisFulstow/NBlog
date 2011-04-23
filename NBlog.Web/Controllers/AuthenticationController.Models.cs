using NBlog.Web.Application;

namespace NBlog.Web.Controllers
{
    public partial class AuthenticationController
    {
        public class LoginModel : LayoutModel
        {
            public string OpenID_Identifier { get; set; }
            public string ReturnUrl { get; set; }
            public string Message { get; set; }
        }
    }
}