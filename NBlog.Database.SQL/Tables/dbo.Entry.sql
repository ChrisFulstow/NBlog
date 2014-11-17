CREATE TABLE [dbo].[Entry]
(
    [Id] int NOT NULL IDENTITY (1, 1) PRIMARY KEY,
	[Slug] [nvarchar](250) NOT NULL,
	[Title] [nvarchar](250) NULL,
	[Author] [nvarchar](250) NULL,
	[DateCreated] [datetime] NULL,
	[Markdown] [nvarchar](max) NULL,
	[IsPublished] [bit] NULL,
	[IsCodePrettified] [bit] NULL
)
