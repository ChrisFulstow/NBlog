CREATE TABLE [dbo].[Config]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY (1, 1),
	[Site] NVARCHAR(250) NOT NULL DEFAULT 'localhost-8080',
	[Theme] NVARCHAR(250) NOT NULL DEFAULT 'Default',
	[MetaDescription] NVARCHAR(250) NOT NULL DEFAULT 'NBlog, an ASP.NET MVC 3.0 blog platform.',
	[Heading] NVARCHAR(250) NOT NULL DEFAULT 'NBlog',
	[Title] NVARCHAR(250) NOT NULL DEFAULT 'NBlog',
	[Tagline] NVARCHAR(250) NOT NULL DEFAULT 'A blog platform for ASP.NET MVC developers',
	[Crossbar] NVARCHAR(250) NOT NULL DEFAULT 'ASP.NET MVC 3.0, Razor, JSON.NET, Markdown, Autofac, AntiXSS, HTML5 & CSS3, OpenID, NSubstitute, ELMAH',
	[ClientId] NVARCHAR(MAX) NOT NULL,
	[ClientSecret] NVARCHAR(MAX) NOT NULL,
	[Admins] NVARCHAR(MAX) NOT NULL DEFAULT 'http://cxfx.myopenid.com/',
	[GoogleAnalyticsId] NVARCHAR(250) NOT NULL DEFAULT '',
	[TwitterUsername] NVARCHAR(250) NOT NULL DEFAULT 'twitter',
	[ContactFormId] INT NOT NULL FOREIGN KEY REFERENCES [ContactForm],
	[CloudId] INT NOT NULL FOREIGN KEY REFERENCES [Cloud],
	[DisqusId] INT NOT NULL FOREIGN KEY REFERENCES [Disqus]
)
