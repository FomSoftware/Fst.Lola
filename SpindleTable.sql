

/****** Object:  Table [dbo].[Spindle]    Script Date: 11-Mar-20 10:09:53 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Spindle](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](100) NULL,
	[ElapsedTimeWorkTotal] [bigint] NULL,
	[ElapsedTimeWork3K] [bigint] NULL,
	[ElapsedTimeWork6K] [bigint] NULL,
	[ElapsedTimeWork9K] [bigint] NULL,
	[ElapsedTimeWork12K] [bigint] NULL,
	[ElapsedTimeWork15K] [bigint] NULL,
	[ElapsedTimeWork18K] [bigint] NULL,
	[ExpectedWorkTime] [bigint] NULL,
	[WorkOverheatingCount] [int] NULL,
	[WorkOverPowerCount] [int] NULL,
	[WorkOverVibratingCount] [int] NULL,
	[InstallDate] [datetime] NULL,
	[MachineId] [int] NULL,
	[ReplacedDate] [datetime] NULL,
	[Serial] [varchar](100) NULL,
	[ToolChangedCount] [int] NULL,
 CONSTRAINT [spindle_PRIMARY] PRIMARY KEY NONCLUSTERED 
(
	[Id] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Spindle] ADD  CONSTRAINT [DF__spindle__Replace__6AEFE058]  DEFAULT ((0)) FOR [ReplacedDate]
GO

ALTER TABLE [dbo].[Spindle] ADD  CONSTRAINT [DF__spindle__ToolCha__6BE40491]  DEFAULT ((0)) FOR [ToolChangedCount]
GO

ALTER TABLE [dbo].[Spindle]  WITH CHECK ADD  CONSTRAINT [FK_Spindle_Machine] FOREIGN KEY([MachineId])
REFERENCES [dbo].[Machine] ([Id])
GO

ALTER TABLE [dbo].[Spindle] CHECK CONSTRAINT [FK_Spindle_Machine]
GO


