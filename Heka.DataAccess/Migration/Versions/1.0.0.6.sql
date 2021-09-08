IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='IncidentCategory')
BEGIN
	CREATE TABLE [dbo].[IncidentCategory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IncidentCategoryCode] [nvarchar](50) NULL,
	[IncidentCategoryName] [nvarchar](150) NULL,
	 CONSTRAINT [PK_IncidentCategory] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='Incident')
BEGIN
	CREATE TABLE [dbo].[Incident](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MachineId] [int] NULL,
	[IncidentCategoryId] [int] NULL,
	[IncidentStatus] [int] NULL,
	[Description] [nvarchar](200) NULL,
	[CreatedDate] [datetime] NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[CreatedUserId] [int] NULL,
	[StartedUserId] [int] NULL,
	[EndUserId] [int] NULL,
	 CONSTRAINT [PK_Incident] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[Incident]  WITH CHECK ADD  CONSTRAINT [FK_Incident_IncidentCategory] FOREIGN KEY([IncidentCategoryId])
	REFERENCES [dbo].[IncidentCategory] ([Id])

	ALTER TABLE [dbo].[Incident] CHECK CONSTRAINT [FK_Incident_IncidentCategory]

	ALTER TABLE [dbo].[Incident]  WITH CHECK ADD  CONSTRAINT [FK_Incident_Machine] FOREIGN KEY([MachineId])
	REFERENCES [dbo].[Machine] ([Id])

	ALTER TABLE [dbo].[Incident] CHECK CONSTRAINT [FK_Incident_Machine]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='PostureCategory')
BEGIN
	CREATE TABLE [dbo].[PostureCategory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PostureCategoryCode] [nvarchar](50) NULL,
	[PostureCategoryName] [nvarchar](250) NULL,
	 CONSTRAINT [PK_PostureCategory] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ProductionPostrue' AND COLUMN_NAME='PostureCategoryId')
BEGIN
	ALTER TABLE ProductionPosture ADD PostureCategoryId INT NULL
	ALTER TABLE ProductionPosture ADD CONSTRAINT FK_ProductionPosture_PostureCategory FOREIGN KEY(PostureCategoryId) REFERENCES PostureCategory(Id)
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Machine' AND COLUMN_NAME='IsWatched')
BEGIN
	ALTER TABLE Machine ADD IsWatched BIT NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Machine' AND COLUMN_NAME='WatchCycleStartCondition')
BEGIN
	ALTER TABLE Machine ADD WatchCycleStartCondition NVARCHAR(300) NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Machine' AND COLUMN_NAME='DeviceIp')
BEGIN
	ALTER TABLE Machine ADD DeviceIp NVARCHAR(100) NULL
END