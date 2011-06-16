using System.Web;

namespace NBlog.Web.Application.Infrastructure
{
    public class HttpTenantSelector
    {
        public string Name
        {
            get
            {
                var hostname = HttpContext.Current.Request.Url.Host;
                
                if (string.IsNullOrWhiteSpace(hostname))
                {
                    hostname = "nblog";
                }

                return hostname.ToUrlSlug();
            }
        }
    }
}