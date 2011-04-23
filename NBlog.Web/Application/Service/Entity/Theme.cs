using System.Web;

namespace NBlog.Web.Application.Service.Entity
{
    public class Theme
    {
        public string Layout { get; set; }
        public string Name { get; set; }
        public string BasePath { get; set; }

        public Theme(string name, string basePath)
        {
            Name = name;
            BasePath = basePath;
            Layout = basePath.UriCombine("/_Layout.cshtml");
        }

        public string Css(string name)
        {
            var path = BasePath.UriCombine(name);
            if (!path.EndsWith("css")) { path = path + ".css"; }
            return path;
        }

        public string Image(string name)
        {
            var path = BasePath.UriCombine("Images").UriCombine(name);
            return path;
        }

        public string Script(string name)
        {
            var path = BasePath.UriCombine("Scripts").UriCombine(name);
            path = VirtualPathUtility.Combine(path, name);
            if (!path.EndsWith("js")) { path = path + ".js"; }
            return path;
        }
    }
}
