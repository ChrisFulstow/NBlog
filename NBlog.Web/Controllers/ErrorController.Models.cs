using NBlog.Web.Application;
using NBlog.Web.Application.Infrastructure;

namespace NBlog.Web.Controllers
{
    public partial class ErrorController
    {
        public class ErrorModel : LayoutModel
        {
            public string Heading { get; set; }
            public string Message { get; set; }
        }
    }
}