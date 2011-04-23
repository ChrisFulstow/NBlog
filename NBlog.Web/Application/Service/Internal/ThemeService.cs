using System;
using NBlog.Web.Application.Service.Entity;

namespace NBlog.Web.Application.Service.Internal
{
    public class ThemeService : IThemeService
    {
        private IConfigService ConfigService { get; set; }

        public ThemeService(IConfigService configService)
        {
            ConfigService = configService;
        }

        public Theme Current
        {
            get
            {
                var themeBasePath = String.Format("~/Resources/Themes/{0}", ConfigService.Current.Theme);
                return new Theme(ConfigService.Current.Theme, themeBasePath);
            }
        }
    }
}