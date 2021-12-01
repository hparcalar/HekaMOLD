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
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='User' AND COLUMN_NAME='ProfileImage')
BEGIN
	ALTER TABLE [User] ADD ProfileImage IMAGE NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Shift' AND COLUMN_NAME='ShiftChiefId')
BEGIN
	ALTER TABLE [Shift] ADD ShiftChiefId INT NULL
	ALTER TABLE [Shift] ADD CONSTRAINT FK_Shift_User FOREIGN KEY(ShiftChiefId) REFERENCES [User](Id)
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='ShiftTarget')
BEGIN
	CREATE TABLE [dbo].[ShiftTarget](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ShiftId] [int] NULL,
	[TargetDate] [datetime] NULL,
	[TargetCount] [int] NULL,
	 CONSTRAINT [PK_ShiftTarget] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[ShiftTarget]  WITH CHECK ADD  CONSTRAINT [FK_ShiftTarget_Shift] FOREIGN KEY([ShiftId])
	REFERENCES [dbo].[Shift] ([Id])

	ALTER TABLE [dbo].[ShiftTarget] CHECK CONSTRAINT [FK_ShiftTarget_Shift]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ShiftTarget' AND COLUMN_NAME='MachineId')
BEGIN
	ALTER TABLE [ShiftTarget] ADD MachineId INT NULL
	ALTER TABLE [ShiftTarget] ADD CONSTRAINT FK_ShiftTarget_Machine FOREIGN KEY(MachineId) REFERENCES Machine(Id)
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='ProcessGroup')
BEGIN
	CREATE TABLE [dbo].[ProcessGroup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProcessGroupCode] [nvarchar](50) NULL,
	[ProcessGroupName] [nvarchar](250) NULL,
	[PlantId] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedUserId] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedUserId] [int] NULL,
	 CONSTRAINT [PK_ProcessGroup] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[ProcessGroup]  WITH CHECK ADD  CONSTRAINT [FK_ProcessGroup_Plant] FOREIGN KEY([PlantId])
	REFERENCES [dbo].[Plant] ([Id])

	ALTER TABLE [dbo].[ProcessGroup] CHECK CONSTRAINT [FK_ProcessGroup_Plant]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='Process')
BEGIN
	CREATE TABLE [dbo].[Process](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProcessCode] [nvarchar](50) NULL,
	[ProcessName] [nvarchar](250) NULL,
	[PlantId] [int] NULL,
	[IsActive] [bit] NULL,
	[TheoreticalDuration] [decimal](18, 5) NULL,
	[ProcessGroupId] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedUserId] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedUserId] [int] NULL,
	 CONSTRAINT [PK_Process] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[Process]  WITH CHECK ADD  CONSTRAINT [FK_Process_Plant] FOREIGN KEY([PlantId])
	REFERENCES [dbo].[Plant] ([Id])

	ALTER TABLE [dbo].[Process] CHECK CONSTRAINT [FK_Process_Plant]

	ALTER TABLE [dbo].[Process]  WITH CHECK ADD  CONSTRAINT [FK_Process_ProcessGroup] FOREIGN KEY([ProcessGroupId])
	REFERENCES [dbo].[ProcessGroup] ([Id])

	ALTER TABLE [dbo].[Process] CHECK CONSTRAINT [FK_Process_ProcessGroup]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='Route')
BEGIN
	CREATE TABLE [dbo].[Route](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RouteCode] [nvarchar](50) NULL,
	[RouteName] [nvarchar](250) NULL,
	[PlantId] [int] NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedUserId] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedUserId] [int] NULL,
	 CONSTRAINT [PK_Route] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[Route]  WITH CHECK ADD  CONSTRAINT [FK_Route_Plant] FOREIGN KEY([PlantId])
	REFERENCES [dbo].[Plant] ([Id])

	ALTER TABLE [dbo].[Route] CHECK CONSTRAINT [FK_Route_Plant]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='RouteItem')
BEGIN
	CREATE TABLE [dbo].[RouteItem](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[RouteId] [int] NULL,
		[ProcessId] [int] NULL,
		[ProcessGroupId] [int] NULL,
		[LineNumber] [int] NULL,
		[Explanation] [nvarchar](250) NULL,
		[MachineId] [int] NULL,
		[MachineGroupId] [int] NULL,
	 CONSTRAINT [PK_RouteItem] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[RouteItem]  WITH CHECK ADD  CONSTRAINT [FK_RouteItem_Machine] FOREIGN KEY([MachineId])
	REFERENCES [dbo].[Machine] ([Id])

	ALTER TABLE [dbo].[RouteItem] CHECK CONSTRAINT [FK_RouteItem_Machine]

	ALTER TABLE [dbo].[RouteItem]  WITH CHECK ADD  CONSTRAINT [FK_RouteItem_MachineGroup] FOREIGN KEY([MachineGroupId])
	REFERENCES [dbo].[MachineGroup] ([Id])

	ALTER TABLE [dbo].[RouteItem] CHECK CONSTRAINT [FK_RouteItem_MachineGroup]

	ALTER TABLE [dbo].[RouteItem]  WITH CHECK ADD  CONSTRAINT [FK_RouteItem_Process] FOREIGN KEY([ProcessId])
	REFERENCES [dbo].[Process] ([Id])

	ALTER TABLE [dbo].[RouteItem] CHECK CONSTRAINT [FK_RouteItem_Process]

	ALTER TABLE [dbo].[RouteItem]  WITH CHECK ADD  CONSTRAINT [FK_RouteItem_ProcessGroup] FOREIGN KEY([ProcessGroupId])
	REFERENCES [dbo].[ProcessGroup] ([Id])

	ALTER TABLE [dbo].[RouteItem] CHECK CONSTRAINT [FK_RouteItem_ProcessGroup]

	ALTER TABLE [dbo].[RouteItem]  WITH CHECK ADD  CONSTRAINT [FK_RouteItem_Route] FOREIGN KEY([RouteId])
	REFERENCES [dbo].[Route] ([Id])

	ALTER TABLE [dbo].[RouteItem] CHECK CONSTRAINT [FK_RouteItem_Route]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='ActualRouteHistory')
BEGIN
	CREATE TABLE [dbo].[ActualRouteHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WorkOrderDetailId] [int] NULL,
	[ProcessId] [int] NULL,
	[ProcessGroupId] [int] NULL,
	[MachineId] [int] NULL,
	[MachineGroupId] [int] NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[ProcessStatus] [int] NULL,
	[StartUserId] [int] NULL,
	[EndUserId] [int] NULL,
	[Explanation] [nvarchar](50) NULL,
	 CONSTRAINT [PK_ActualRouteHistory] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[ActualRouteHistory]  WITH CHECK ADD  CONSTRAINT [FK_ActualRouteHistory_Machine] FOREIGN KEY([MachineId])
	REFERENCES [dbo].[Machine] ([Id])

	ALTER TABLE [dbo].[ActualRouteHistory] CHECK CONSTRAINT [FK_ActualRouteHistory_Machine]

	ALTER TABLE [dbo].[ActualRouteHistory]  WITH CHECK ADD  CONSTRAINT [FK_ActualRouteHistory_MachineGroup] FOREIGN KEY([MachineGroupId])
	REFERENCES [dbo].[MachineGroup] ([Id])

	ALTER TABLE [dbo].[ActualRouteHistory] CHECK CONSTRAINT [FK_ActualRouteHistory_MachineGroup]

	ALTER TABLE [dbo].[ActualRouteHistory]  WITH CHECK ADD  CONSTRAINT [FK_ActualRouteHistory_Process] FOREIGN KEY([ProcessId])
	REFERENCES [dbo].[Process] ([Id])

	ALTER TABLE [dbo].[ActualRouteHistory] CHECK CONSTRAINT [FK_ActualRouteHistory_Process]

	ALTER TABLE [dbo].[ActualRouteHistory]  WITH CHECK ADD  CONSTRAINT [FK_ActualRouteHistory_ProcessGroup] FOREIGN KEY([ProcessGroupId])
	REFERENCES [dbo].[ProcessGroup] ([Id])

	ALTER TABLE [dbo].[ActualRouteHistory] CHECK CONSTRAINT [FK_ActualRouteHistory_ProcessGroup]

	ALTER TABLE [dbo].[ActualRouteHistory]  WITH CHECK ADD  CONSTRAINT [FK_ActualRouteHistory_WorkOrderDetail] FOREIGN KEY([WorkOrderDetailId])
	REFERENCES [dbo].[WorkOrderDetail] ([Id])

	ALTER TABLE [dbo].[ActualRouteHistory] CHECK CONSTRAINT [FK_ActualRouteHistory_WorkOrderDetail]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Machine' AND COLUMN_NAME='SignalEndDelay')
BEGIN
	ALTER TABLE Machine ADD SignalEndDelay INT NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='PostureCategory' AND COLUMN_NAME='ShouldStopSignal')
BEGIN
	ALTER TABLE PostureCategory ADD ShouldStopSignal BIT NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='WorkOrderSerial' AND COLUMN_NAME='TargetWarehouseId')
BEGIN
	ALTER TABLE WorkOrderSerial ADD TargetWarehouseId INT NULL
	ALTER TABLE WorkOrderSerial ADD CONSTRAINT FK_WorkOrderSerial_Warehouse FOREIGN KEY(TargetWarehouseId) REFERENCES Warehouse(Id)
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='MachineGroup' AND COLUMN_NAME='IsProduction')
BEGIN
	ALTER TABLE MachineGroup ADD IsProduction BIT NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='WorkOrderCategory')
BEGIN
	CREATE TABLE [dbo].[WorkOrderCategory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WorkOrderCategoryCode] [nvarchar](50) NULL,
	[WorkOrderCategoryName] [nvarchar](250) NULL,
	 CONSTRAINT [PK_WorkOrderCategory] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='WorkOrder' AND COLUMN_NAME='WorkOrderCategoryId')
BEGIN
	ALTER TABLE WorkOrder ADD WorkOrderCategoryId INT NULL
	ALTER TABLE WorkOrder ADD CONSTRAINT FK_WorkOrder_WorkOrderCategory FOREIGN KEY(WorkOrderCategoryId) REFERENCES WorkOrderCategory(Id)
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ItemSerial' AND COLUMN_NAME='FirstQuantity')
BEGIN
	ALTER TABLE ItemSerial ADD FirstQuantity DECIMAL(18,5) NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ItemSerial' AND COLUMN_NAME='LiveQuantity')
BEGIN
	ALTER TABLE ItemSerial ADD LiveQuantity DECIMAL(18,5) NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ItemSerial' AND COLUMN_NAME='WorkOrderDetailId')
BEGIN
	ALTER TABLE ItemSerial ADD WorkOrderDetailId INT NULL
	ALTER TABLE ItemSerial ADD CONSTRAINT FK_ItemSerial_WorkOrderDetail FOREIGN KEY(WorkOrderDetailId) REFERENCES WorkOrderDetail(Id)
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ItemSerial' AND COLUMN_NAME='SerialType')
BEGIN
	ALTER TABLE ItemSerial ADD SerialType INT NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ItemSerial' AND COLUMN_NAME='InPackageQuantity')
BEGIN
	ALTER TABLE ItemSerial ADD InPackageQuantity INT NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='WorkOrderCategory')
BEGIN
	CREATE TABLE [dbo].[ItemOrderConsume](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ItemOrderDetailId] [int] NULL,
	[ConsumerReceiptDetailId] [int] NULL,
	[ConsumedReceiptDetailId] [int] NULL,
	[UsedQuantity] [decimal](18, 5) NULL,
	[UsedGrossQuantity] [decimal](18, 5) NULL,
	[UnitId] [int] NULL,
	 CONSTRAINT [PK_ItemOrderConsume] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[ItemOrderConsume]  WITH CHECK ADD  CONSTRAINT [FK_ItemOrderConsume_ItemOrderDetail] FOREIGN KEY([ItemOrderDetailId])
	REFERENCES [dbo].[ItemOrderDetail] ([Id])

	ALTER TABLE [dbo].[ItemOrderConsume] CHECK CONSTRAINT [FK_ItemOrderConsume_ItemOrderDetail]

	ALTER TABLE [dbo].[ItemOrderConsume]  WITH CHECK ADD  CONSTRAINT [FK_ItemOrderConsume_ItemReceiptDetailConsumed] FOREIGN KEY([ConsumedReceiptDetailId])
	REFERENCES [dbo].[ItemReceiptDetail] ([Id])

	ALTER TABLE [dbo].[ItemOrderConsume] CHECK CONSTRAINT [FK_ItemOrderConsume_ItemReceiptDetailConsumed]

	ALTER TABLE [dbo].[ItemOrderConsume]  WITH CHECK ADD  CONSTRAINT [FK_ItemOrderConsume_ItemReceiptDetailConsumer] FOREIGN KEY([ConsumerReceiptDetailId])
	REFERENCES [dbo].[ItemReceiptDetail] ([Id])

	ALTER TABLE [dbo].[ItemOrderConsume] CHECK CONSTRAINT [FK_ItemOrderConsume_ItemReceiptDetailConsumer]

	ALTER TABLE [dbo].[ItemOrderConsume]  WITH CHECK ADD  CONSTRAINT [FK_ItemOrderConsume_UnitType] FOREIGN KEY([UnitId])
	REFERENCES [dbo].[UnitType] ([Id])

	ALTER TABLE [dbo].[ItemOrderConsume] CHECK CONSTRAINT [FK_ItemOrderConsume_UnitType]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='WorkOrderDetail' AND COLUMN_NAME='LabelConfig')
BEGIN
	ALTER TABLE WorkOrderDetail ADD LabelConfig NVARCHAR(300) NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ProductWastage' AND COLUMN_NAME='IsAfterScrap')
BEGIN
	ALTER TABLE ProductWastage ADD IsAfterScrap BIT NULL
END