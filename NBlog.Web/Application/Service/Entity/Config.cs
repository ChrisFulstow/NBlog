using Newtonsoft.Json;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace NBlog.Web.Application.Service.Entity
{
	[PrimaryKey("Id")]
	public class Config
	{
		private readonly bool isSqlRepositoryType = ConfigurationManager.AppSettings["RepositoryType"].Equals("sql", StringComparison.InvariantCultureIgnoreCase);

		[Column("Admins")]
		public string AdminsCsv { get; set; }

		[JsonProperty("Admins")]
		private List<string> _admins;

		[Ignore]
		[JsonIgnore]
		public List<string> Admins
		{
			get
			{
				if (!isSqlRepositoryType)
				{
					return _admins;
				}
				else
				{
					return AdminsCsv.Split(',').ToList();
				}
			}
			set
			{
				_admins = value;
			}
		}

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