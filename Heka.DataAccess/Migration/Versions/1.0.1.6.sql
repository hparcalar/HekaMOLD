IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='MachineMaintenanceInstruction')
BEGIN
	CREATE TABLE [dbo].[MachineMaintenanceInstruction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MachineId] [int] NULL,
	[UnitName] [nvarchar](75) NULL,
	[PeriodType] [nvarchar](5) NULL,
	[ToDoList] [nvarchar](max) NULL,
	[Responsible] [nvarchar](150) NULL,
	[LineNumber] [int] NULL,
	 CONSTRAINT [PK_MachineMaintenanceInstruction] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

	ALTER TABLE [dbo].[MachineMaintenanceInstruction]  WITH CHECK ADD  CONSTRAINT [FK_MachineMaintenanceInstruction_Machine] FOREIGN KEY([MachineId])
	REFERENCES [dbo].[Machine] ([Id])

	ALTER TABLE [dbo].[MachineMaintenanceInstruction] CHECK CONSTRAINT [FK_MachineMaintenanceInstruction_Machine]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='MachineMaintenanceInstructionEntry')
BEGIN
	CREATE TABLE [dbo].[MachineMaintenanceInstructionEntry](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InstructionId] [int] NULL,
	[CreatedUserId] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[IsChecked] [bit] NULL,
	[Explanation] [nvarchar](250) NULL,
	 CONSTRAINT [PK_MachineMaintenanceInstructionEntry] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[MachineMaintenanceInstructionEntry]  WITH CHECK ADD  CONSTRAINT [FK_MachineMaintenanceInstructionEntry_MachineMaintenanceInstruction] FOREIGN KEY([InstructionId])
	REFERENCES [dbo].[MachineMaintenanceInstruction] ([Id])

	ALTER TABLE [dbo].[MachineMaintenanceInstructionEntry] CHECK CONSTRAINT [FK_MachineMaintenanceInstructionEntry_MachineMaintenanceInstruction]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='MoldTest' AND COLUMN_NAME='HeadSize')
BEGIN
	ALTER TABLE MoldTest ADD HeadSize NVARCHAR(50) NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='EntryQualityPlan')
BEGIN
	CREATE TABLE [dbo].[EntryQualityPlan](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[QualityControlCode] [nvarchar](50) NULL,
		[OrderNo] [int] NULL,
		[ItemGroupId] [int] NULL,
		[ItemCategoryId] [int] NULL,
		[ItemGroupText] [nvarchar](200) NULL,
		[PeriodType] [nvarchar](50) NULL,
		[AcceptanceCriteria] [nvarchar](250) NULL,
		[ControlDevice] [nvarchar](150) NULL,
		[Method] [nvarchar](150) NULL,
		[Responsible] [nvarchar](100) NULL,
		[RecordType] [nvarchar](150) NULL,
	 CONSTRAINT [PK_EntryQualityPlan] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[EntryQualityPlan]  WITH CHECK ADD  CONSTRAINT [FK_EntryQualityPlan_ItemCategory] FOREIGN KEY([ItemCategoryId])
	REFERENCES [dbo].[ItemCategory] ([Id])

	ALTER TABLE [dbo].[EntryQualityPlan] CHECK CONSTRAINT [FK_EntryQualityPlan_ItemCategory]

	ALTER TABLE [dbo].[EntryQualityPlan]  WITH CHECK ADD  CONSTRAINT [FK_EntryQualityPlan_ItemGroup] FOREIGN KEY([ItemGroupId])
	REFERENCES [dbo].[ItemGroup] ([Id])

	ALTER TABLE [dbo].[EntryQualityPlan] CHECK CONSTRAINT [FK_EntryQualityPlan_ItemGroup]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='EntryQualityPlanDetail')
BEGIN
	CREATE TABLE [dbo].[EntryQualityPlanDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EntryQualityPlanId] [int] NULL,
	[CheckProperty] [nvarchar](250) NULL,
	[IsRequired] [bit] NULL,
	[OrderNo] [int] NULL,
	 CONSTRAINT [PK_EntryQualityPlanDetail] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[EntryQualityPlanDetail]  WITH CHECK ADD  CONSTRAINT [FK_EntryQualityPlanDetail_EntryQualityPlan] FOREIGN KEY([EntryQualityPlanId])
	REFERENCES [dbo].[EntryQualityPlan] ([Id])

	ALTER TABLE [dbo].[EntryQualityPlanDetail] CHECK CONSTRAINT [FK_EntryQualityPlanDetail_EntryQualityPlan]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='EntryQualityData')
BEGIN
	CREATE TABLE [dbo].[EntryQualityData](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[QualityPlanId] [int] NULL,
	[QualityPlanDetailId] [int] NULL,
	[IsOk] [bit] NULL,
	[Explanation] [nvarchar](200) NULL,
	[ItemEntryDetailId] [int] NULL,
	[ItemLot] [nvarchar](50) NULL,
	[ItemNo] [nvarchar](50) NULL,
	[ItemName] [nvarchar](150) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedUserId] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedUserId] [int] NULL,
	 CONSTRAINT [PK_EntryQualityData] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[EntryQualityData]  WITH CHECK ADD  CONSTRAINT [FK_EntryQualityData_EntryQualityPlan] FOREIGN KEY([QualityPlanId])
	REFERENCES [dbo].[EntryQualityPlan] ([Id])

	ALTER TABLE [dbo].[EntryQualityData] CHECK CONSTRAINT [FK_EntryQualityData_EntryQualityPlan]

	ALTER TABLE [dbo].[EntryQualityData]  WITH CHECK ADD  CONSTRAINT [FK_EntryQualityData_EntryQualityPlanDetail] FOREIGN KEY([QualityPlanDetailId])
	REFERENCES [dbo].[EntryQualityPlanDetail] ([Id])

	ALTER TABLE [dbo].[EntryQualityData] CHECK CONSTRAINT [FK_EntryQualityData_EntryQualityPlanDetail]

	ALTER TABLE [dbo].[EntryQualityData]  WITH CHECK ADD  CONSTRAINT [FK_EntryQualityData_ItemReceiptDetail] FOREIGN KEY([ItemEntryDetailId])
	REFERENCES [dbo].[ItemReceiptDetail] ([Id])

	ALTER TABLE [dbo].[EntryQualityData] CHECK CONSTRAINT [FK_EntryQualityData_ItemReceiptDetail]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='Shift')
BEGIN
	CREATE TABLE [dbo].[Shift](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ShiftCode] [nvarchar](50) NULL,
	[ShiftName] [nvarchar](50) NULL,
	[StartTime] [time](7) NULL,
	[EndTime] [time](7) NULL,
	[IsActive] [bit] NULL,
	 CONSTRAINT [PK_Shift] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='WorkOrderSerial' AND COLUMN_NAME='ShiftId')
BEGIN
	ALTER TABLE WorkOrderSerial ADD ShiftId INT NULL
	ALTER TABLE WorkOrderSerial ADD CONSTRAINT FK_WorkOrderSerial_Shift FOREIGN KEY(ShiftId) REFERENCES Shift(Id)
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='EntryQualityPlanDetail' AND COLUMN_NAME='PeriodType')
BEGIN
	ALTER TABLE EntryQualityPlanDetail ADD PeriodType NVARCHAR(50) NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='EntryQualityPlanDetail' AND COLUMN_NAME='AcceptanceCriteria')
BEGIN
	ALTER TABLE EntryQualityPlanDetail ADD AcceptanceCriteria NVARCHAR(250) NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='EntryQualityPlanDetail' AND COLUMN_NAME='ControlDevice')
BEGIN
	ALTER TABLE EntryQualityPlanDetail ADD ControlDevice NVARCHAR(150) NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='EntryQualityPlanDetail' AND COLUMN_NAME='Method')
BEGIN
	ALTER TABLE EntryQualityPlanDetail ADD Method NVARCHAR(150) NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='EntryQualityPlanDetail' AND COLUMN_NAME='Responsible')
BEGIN
	ALTER TABLE EntryQualityPlanDetail ADD Responsible NVARCHAR(100) NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='ProductQualityPlan')
BEGIN
	CREATE TABLE [dbo].[ProductQualityPlan](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductQualityCode] [nvarchar](150) NULL,
	[CheckProperties] [nvarchar](500) NULL,
	[PeriodType] [nvarchar](50) NULL,
	[AcceptanceCriteria] [nvarchar](150) NULL,
	[ControlDevice] [nvarchar](100) NULL,
	[Method] [nvarchar](150) NULL,
	[Responsible] [nvarchar](150) NULL,
	 CONSTRAINT [PK_ProductQualityPlan] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ProductQualityPlan' AND COLUMN_NAME='OrderNo')
BEGIN
	ALTER TABLE ProductQualityPlan ADD OrderNo INT NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ProductQualityPlan' AND COLUMN_NAME='CheckType')
BEGIN
	ALTER TABLE ProductQualityPlan ADD CheckType INT NOT NULL DEFAULT 1
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ProductQualityPlan' AND COLUMN_NAME='ToleranceMin')
BEGIN
	ALTER TABLE ProductQualityPlan ADD ToleranceMin DECIMAL(18,5) NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ProductQualityPlan' AND COLUMN_NAME='ToleranceMax')
BEGIN
	ALTER TABLE ProductQualityPlan ADD ToleranceMax DECIMAL(18,5) NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='ProductQualityData')
BEGIN
	CREATE TABLE [dbo].[ProductQualityData](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MachineId] [int] NULL,
	[ProductId] [int] NULL,
	[ControlDate] [datetime] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedUserId] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedUserId] [int] NULL,
	 CONSTRAINT [PK_ProductQualityData] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[ProductQualityData]  WITH CHECK ADD  CONSTRAINT [FK_ProductQualityData_Item] FOREIGN KEY([ProductId])
	REFERENCES [dbo].[Item] ([Id])

	ALTER TABLE [dbo].[ProductQualityData] CHECK CONSTRAINT [FK_ProductQualityData_Item]

	ALTER TABLE [dbo].[ProductQualityData]  WITH CHECK ADD  CONSTRAINT [FK_ProductQualityData_Machine] FOREIGN KEY([MachineId])
	REFERENCES [dbo].[Machine] ([Id])

	ALTER TABLE [dbo].[ProductQualityData] CHECK CONSTRAINT [FK_ProductQualityData_Machine]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='ProductQualityDataDetail')
BEGIN
	CREATE TABLE [dbo].[ProductQualityDataDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductQualityDataId] [int] NULL,
	[ProductQualityPlanId] [int] NULL,
	[OrderNo] [int] NULL,
	[CheckResult] [bit] NULL,
	[NumericResult] [decimal](18, 5) NULL,
	[IsOk] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedUserId] [int] NULL,
	 CONSTRAINT [PK_ProductQualityDataDetail] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[ProductQualityDataDetail]  WITH CHECK ADD  CONSTRAINT [FK_ProductQualityDataDetail_ProductQualityData] FOREIGN KEY([ProductQualityDataId])
	REFERENCES [dbo].[ProductQualityData] ([Id])

	ALTER TABLE [dbo].[ProductQualityDataDetail] CHECK CONSTRAINT [FK_ProductQualityDataDetail_ProductQualityData]

	ALTER TABLE [dbo].[ProductQualityDataDetail]  WITH CHECK ADD  CONSTRAINT [FK_ProductQualityDataDetail_ProductQualityPlan] FOREIGN KEY([ProductQualityPlanId])
	REFERENCES [dbo].[ProductQualityPlan] ([Id])

	ALTER TABLE [dbo].[ProductQualityDataDetail] CHECK CONSTRAINT [FK_ProductQualityDataDetail_ProductQualityPlan]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='EntryQualityData' AND COLUMN_NAME='FirmId')
BEGIN
	ALTER TABLE EntryQualityData ADD FirmId INT NULL
	ALTER TABLE EntryQualityData ADD CONSTRAINT FK_EntryQualityData_Firm FOREIGN KEY(FirmId) REFERENCES Firm(Id)
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='EntryQualityData' AND COLUMN_NAME='ItemId')
BEGIN
	ALTER TABLE EntryQualityData ADD ItemId INT NULL
	ALTER TABLE EntryQualityData ADD CONSTRAINT FK_EntryQualityData_Item FOREIGN KEY(ItemId) REFERENCES Item(Id)
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='EntryQualityData' AND COLUMN_NAME='WaybillNo')
BEGIN
	ALTER TABLE EntryQualityData ADD WaybillNo NVARCHAR(150) NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='EntryQualityData' AND COLUMN_NAME='EntryQuantity')
BEGIN
	ALTER TABLE EntryQualityData ADD EntryQuantity DECIMAL(18,5) NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='EntryQualityData' AND COLUMN_NAME='CheckedQuantity')
BEGIN
	ALTER TABLE EntryQualityData ADD CheckedQuantity DECIMAL(18,5) NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='EntryQualityData' AND COLUMN_NAME='LotNumbers')
BEGIN
	ALTER TABLE EntryQualityData ADD LotNumbers NVARCHAR(300) NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='EntryQualityData' AND COLUMN_NAME='SampleQuantity')
BEGIN
	ALTER TABLE EntryQualityData ADD SampleQuantity DECIMAL(18,5) NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='EntryQualityDataDetail')
BEGIN
	CREATE TABLE [dbo].[EntryQualityDataDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EntryQualityDataId] [int] NULL,
	[EntryQualityPlanDetailId] [int] NULL,
	[OrderNo] [int] NULL,
	[FaultExplanation] [nvarchar](300) NULL,
	[SampleQuantity] [decimal](18,5) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedUserId] [int] NULL,
	 CONSTRAINT [PK_EntryQualityDataDetail] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[EntryQualityDataDetail]  WITH CHECK ADD  CONSTRAINT [FK_EntryQualityDataDetail_EntryQualityData] FOREIGN KEY([EntryQualityDataId])
	REFERENCES [dbo].[EntryQualityData] ([Id])

	ALTER TABLE [dbo].[EntryQualityDataDetail] CHECK CONSTRAINT [FK_EntryQualityDataDetail_EntryQualityData]

	ALTER TABLE [dbo].[EntryQualityDataDetail]  WITH CHECK ADD  CONSTRAINT [FK_EntryQualityDataDetail_EntryQualityPlanDetail] FOREIGN KEY([EntryQualityPlanDetailId])
	REFERENCES [dbo].[EntryQualityPlanDetail] ([Id])

	ALTER TABLE [dbo].[EntryQualityDataDetail] CHECK CONSTRAINT [FK_EntryQualityDataDetail_EntryQualityPlanDetail]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='UsageDocument')
BEGIN
	CREATE TABLE [dbo].[UsageDocument](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[DocumentTitle] [nvarchar](300) NULL,
		[DocumentData] [nvarchar](max) NULL,
		[CreatedDate] [datetime] NULL,
		[CreatedUserId] [int] NULL,
		[UpdatedDate] [datetime] NULL,
		[UpdatedUserId] [int] NULL,
	 CONSTRAINT [PK_UsageDocument] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END