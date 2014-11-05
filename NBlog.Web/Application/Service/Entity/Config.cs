using PetaPoco;
using System.Collections.Generic;
using System.Linq;

namespace NBlog.Web.Application.Service.Entity
{
	[TableName("Config")]
	[PrimaryKey("Id")]
	public class Config
	{
		[Column("Admins")]
		public string AdminsCsvString { get; set; }

		[Ignore]
		public List<string> Admins { get { return AdminsCsvString.Split(',').ToList(); } }

		public int CloudId { get; set; }

		[ResultColumn]
		public CloudConfig Cloud { get; set; }

		public int ContactFormId { get; set; }

		[ResultColumn]
		public ContactFormConfig ContactForm { get; set; }

		public string Crossbar { get; set; }

		public int DisqusId { get; set; }

		[ResultColumn]
		public DisqusConfig Disqus { get; set; }

		public string GoogleAnalyticsId { get; set; }

		public string Heading { get; set; }

		public string MetaDescription { get; set; }

		public string Site { get; set; }

		public string Tagline { get; set; }

		public string Theme { get; set; }

		public string Title { get; set; }

		public string TwitterUsername { get; set; }

		public string ClientId { get; set; }

		public string ClientSecret { get; set; }

		[TableName("Cloud")]
		[PrimaryKey("Id")]
		public class CloudConfig
		{
			public string ConsumerKey { get; set; }

			public string ConsumerSecret { get; set; }

			public string UserSecret { get; set; }

			public string UserToken { get; set; }
		}

		[TableName("ContactForm")]
		[PrimaryKey("Id")]
		public class ContactFormConfig
		{
			public string RecipientEmail { get; set; }

			public string RecipientName { get; set; }

			public string Subject { get; set; }
		}

		[TableName("Disqus")]
		[PrimaryKey("Id")]
		public class DisqusConfig
		{
			public bool DevelopmentMode { get; set; }

			public string Shortname { get; set; }
		}
	}
}