CREATE TABLE [dbo].[Cloud]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY (1, 1),
	[ConsumerKey] NVARCHAR(250) NOT NULL DEFAULT 'DropBox_Key',
	[ConsumerSecret] NVARCHAR(250) NOT NULL DEFAULT 'DropBox_Secret',
	[UserToken] NVARCHAR(250) NOT NULL DEFAULT 'DropBox_UserToken',
	[UserSecret] NVARCHAR(250) NOT NULL DEFAULT 'DropBox_UserSecret'
)
