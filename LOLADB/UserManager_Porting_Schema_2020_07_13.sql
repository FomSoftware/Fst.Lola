USE [LOLATEST]
GO
/****** Object:  Table [dbo].[AuditLogin]    Script Date: 13-Jul-20 3:06:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditLogin](
	[UserID] [uniqueidentifier] NULL,
	[DateAndTime] [datetime] NOT NULL,
	[Accessed] [bit] NULL,
	[MessageInfo] [nvarchar](max) NULL,
	[IP] [nvarchar](20) NULL,
	[Username] [nvarchar](200) NULL,
	[ID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_AuditLogin] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Languages]    Script Date: 13-Jul-20 3:06:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Languages](
	[ID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](15) NOT NULL,
	[InitialsLanguage] [nvarchar](5) NOT NULL,
	[IdLanguage] [int] NULL,
	[DotNetCulture] [nvarchar](50) NULL,
 CONSTRAINT [PK_Language] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 13-Jul-20 3:06:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[ID] [uniqueidentifier] NOT NULL,
	[IdRole] [int] NULL,
	[Name] [nvarchar](500) NULL,
	[Description] [nvarchar](1500) NULL,
	[Enabled] [bit] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedDate] [datetime] NULL,
	[Status] [int] NULL,
	[HomePage] [nvarchar](max) NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles_Users]    Script Date: 13-Jul-20 3:06:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles_Users](
	[ID] [uniqueidentifier] NOT NULL,
	[RoleID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Roles_Users] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 13-Jul-20 3:06:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[ID] [uniqueidentifier] NOT NULL,
	[Username] [nvarchar](200) NULL,
	[Password] [nvarchar](max) NULL,
	[FirstName] [nvarchar](250) NULL,
	[LastName] [nvarchar](250) NULL,
	[LanguageID] [uniqueidentifier] NULL,
	[Email] [nvarchar](500) NULL,
	[DefaultHomePage] [nvarchar](1000) NULL,
	[Enabled] [bit] NOT NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedDate] [datetime] NULL,
	[Status] [int] NULL,
	[Domain] [nvarchar](max) NULL,
	[LastDateUpdatePassword] [datetime] NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[AuditLogin] ADD  CONSTRAINT [DF_AuditLogin_ID]  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [dbo].[Languages] ADD  CONSTRAINT [DF_Language_ID]  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [dbo].[Roles] ADD  CONSTRAINT [DF_Roles_ID]  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [dbo].[Roles_Users] ADD  CONSTRAINT [DF_Roles_Users_ID]  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_ID]  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_LastDateUpdatePassword]  DEFAULT (NULL) FOR [LastDateUpdatePassword]
GO
ALTER TABLE [dbo].[AuditLogin]  WITH CHECK ADD  CONSTRAINT [FK_AuditLogin_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AuditLogin] CHECK CONSTRAINT [FK_AuditLogin_Users]
GO
ALTER TABLE [dbo].[Roles_Users]  WITH CHECK ADD  CONSTRAINT [FK_Roles_Users_Roles] FOREIGN KEY([RoleID])
REFERENCES [dbo].[Roles] ([ID])
GO
ALTER TABLE [dbo].[Roles_Users] CHECK CONSTRAINT [FK_Roles_Users_Roles]
GO
ALTER TABLE [dbo].[Roles_Users]  WITH CHECK ADD  CONSTRAINT [FK_Roles_Users_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Roles_Users] CHECK CONSTRAINT [FK_Roles_Users_Users]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Languages] FOREIGN KEY([LanguageID])
REFERENCES [dbo].[Languages] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Languages]
GO
