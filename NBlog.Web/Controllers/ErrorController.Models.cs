using NBlog.Web.Application;

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