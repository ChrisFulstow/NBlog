CREATE TABLE [dbo].[ContactForm]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY (1, 1),
	[RecipientName] NVARCHAR(250) NOT NULL DEFAULT 'NBlog Admin',
	[RecipientEmail] NVARCHAR(250) NOT NULL DEFAULT 'nblog-admin@mailinator.com',
	[Subject] NVARCHAR(250) NOT NULL DEFAULT 'NBlog contact form'
)
