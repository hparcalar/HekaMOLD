IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='LayoutObjectType')
BEGIN
	CREATE TABLE [dbo].[LayoutObjectType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ObjectTypeCode] [nvarchar](50) NULL,
	[ObjectTypeName] [nvarchar](300) NULL,
	[ObjectData] [image] NULL,
	[DataTypeExtension] [nvarchar](5) NULL,
	 CONSTRAINT [PK_LayoutObjectType] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='MachineGroup')
BEGIN
	CREATE TABLE [dbo].[MachineGroup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MachineGroupCode] [nvarchar](50) NULL,
	[MachineGroupName] [nvarchar](250) NULL,
	[PlantId] [int] NULL,
	[LayoutObjectTypeId] [int] NULL,
	 CONSTRAINT [PK_MachineGroup] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	) ON [PRIMARY]

	ALTER TABLE [dbo].[MachineGroup]  WITH CHECK ADD  CONSTRAINT [FK_MachineGroup_LayoutObjectType] FOREIGN KEY([LayoutObjectTypeId])
	REFERENCES [dbo].[LayoutObjectType] ([Id])

	ALTER TABLE [dbo].[MachineGroup] CHECK CONSTRAINT [FK_MachineGroup_LayoutObjectType]

	ALTER TABLE [dbo].[MachineGroup]  WITH CHECK ADD  CONSTRAINT [FK_MachineGroup_Plant] FOREIGN KEY([PlantId])
	REFERENCES [dbo].[Plant] ([Id])

	ALTER TABLE [dbo].[MachineGroup] CHECK CONSTRAINT [FK_MachineGroup_Plant]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='LayoutItem')
BEGIN
	CREATE TABLE [dbo].[LayoutItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MachineId] [int] NULL,
	[PositionData] [nvarchar](500) NULL,
	[RotationData] [nvarchar](500) NULL,
	[ScalingData] [nvarchar](500) NULL,
	[Title] [nvarchar](100) NULL,
	[PlantId] [int] NULL,
	 CONSTRAINT [PK_LayoutItem] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[LayoutItem]  WITH CHECK ADD  CONSTRAINT [FK_LayoutItem_Machine] FOREIGN KEY([MachineId])
	REFERENCES [dbo].[Machine] ([Id])

	ALTER TABLE [dbo].[LayoutItem] CHECK CONSTRAINT [FK_LayoutItem_Machine]

	ALTER TABLE [dbo].[LayoutItem]  WITH CHECK ADD  CONSTRAINT [FK_LayoutItem_Plant] FOREIGN KEY([PlantId])
	REFERENCES [dbo].[Plant] ([Id])

	ALTER TABLE [dbo].[LayoutItem] CHECK CONSTRAINT [FK_LayoutItem_Plant]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Machine' AND COLUMN_NAME='MachineGroupId')
BEGIN
	ALTER TABLE Machine ADD MachineGroupId INT NULL
	ALTER TABLE Machine ADD CONSTRAINT FK_Machine_MachineGrouo FOREIGN KEY(MachineGroupId) REFERENCES MachineGroup(Id)
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Plant' AND COLUMN_NAME='LogoData')
BEGIN
	ALTER TABLE Plant ADD LogoData IMAGE NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='UserRoleSubscription')
BEGIN
	CREATE TABLE [dbo].[UserRoleSubscription](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserRoleId] [int] NULL,
	[SubscriptionCategory] [int] NULL,
	 CONSTRAINT [PK_UserSubscription] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[UserRoleSubscription]  WITH CHECK ADD  CONSTRAINT [FK_UserSubscription_UserRole] FOREIGN KEY([UserRoleId])
	REFERENCES [dbo].[UserRole] ([Id])

	ALTER TABLE [dbo].[UserRoleSubscription] CHECK CONSTRAINT [FK_UserSubscription_UserRole]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Notification' AND COLUMN_NAME='PushStatus')
BEGIN
	ALTER TABLE Notification ADD PushStatus INT NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='WorkOrder' AND COLUMN_NAME='WorkOrderType')
BEGIN
	ALTER TABLE WorkOrder ADD WorkOrderType INT NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='WorkOrderDetail' AND COLUMN_NAME='WorkOrderType')
BEGIN
	ALTER TABLE WorkOrderDetail ADD WorkOrderType INT NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='WorkOrder' AND COLUMN_NAME='TrialFirmName')
BEGIN
	ALTER TABLE WorkOrder ADD TrialFirmName NVARCHAR(250) NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='WorkOrderDetail' AND COLUMN_NAME='TrialProductName')
BEGIN
	ALTER TABLE WorkOrderDetail ADD TrialProductName NVARCHAR(300) NULL
END