USE [master]
GO

CREATE DATABASE [Tokens]
GO


USE [Tokens]
GO

/****** Object:  Table [dbo].[Token]    Script Date: 7/2/2019 1:12:02 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Token](
	[RealmId] [nvarchar](50) NOT NULL,
	[AccessToken] [nvarchar](1000) NOT NULL,
	[RefreshToken] [nvarchar](1000) NOT NULL,
 CONSTRAINT [PK_Token] PRIMARY KEY CLUSTERED 
(
	[RealmId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

