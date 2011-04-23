using System.Collections.Generic;
using Newtonsoft.Json;

namespace NBlog.Web.Application.Service.Entity
{
    public class Config
    {
        public string Site { get; set; }
        public string Theme { get; set; }
        public string Title { get; set; }
        public string MetaDescription { get; set; }
        public string Heading { get; set; }
        public string Tagline { get; set; }
        public string Crossbar { get; set; }
        public List<string> Admins { get; set; }
        public string GoogleAnalyticsId { get; set; }
        public string TwitterUsername { get; set; }
        public ContactFormConfig ContactForm { get; set; }
        public CloudConfig Cloud { get; set; }
        public DisqusConfig Disqus { get; set; }

        public class ContactFormConfig
        {
            public string RecipientName { get; set; }
            public string RecipientEmail { get; set; }
            public string Subject { get; set; }
        }

        public class CloudConfig
        {
            public string ConsumerKey { get; set; }
            public string ConsumerSecret { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        public class DisqusConfig
        {
            public string Shortname { get; set; }
            public bool DevelopmentMode { get; set; }
        }
    }
}