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
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='SectionSetting')
BEGIN
	CREATE TABLE [dbo].[SectionSetting](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[ColorCode] [nvarchar](50) NULL,
		[SectionGroupCode] [nvarchar](50) NULL,
	 CONSTRAINT [PK_SectionSetting] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='MoldProduct')
BEGIN
	CREATE TABLE [dbo].[MoldProduct](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MoldId] [int] NULL,
	[ProductId] [int] NULL,
	[LineNumber] [int] NULL,
	 CONSTRAINT [PK_MoldProduct] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[MoldProduct]  WITH CHECK ADD  CONSTRAINT [FK_MoldProduct_Item] FOREIGN KEY([ProductId])
	REFERENCES [dbo].[Item] ([Id])

	ALTER TABLE [dbo].[MoldProduct] CHECK CONSTRAINT [FK_MoldProduct_Item]

	ALTER TABLE [dbo].[MoldProduct]  WITH CHECK ADD  CONSTRAINT [FK_MoldProduct_Mold] FOREIGN KEY([MoldId])
	REFERENCES [dbo].[Mold] ([Id])

	ALTER TABLE [dbo].[MoldProduct] CHECK CONSTRAINT [FK_MoldProduct_Mold]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Mold' AND COLUMN_NAME='FirmId')
BEGIN
	ALTER TABLE Mold ADD FirmId INT NULL
	ALTER TABLE Mold ADD CONSTRAINT FK_Mold_Firm FOREIGN KEY(FirmId) REFERENCES Firm(Id)
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Mold' AND COLUMN_NAME='LifeTimeTicks')
BEGIN
	ALTER TABLE Mold ADD LifeTimeTicks INT NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Mold' AND COLUMN_NAME='CurrentTicks')
BEGIN
	ALTER TABLE Mold ADD CurrentTicks INT NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Mold' AND COLUMN_NAME='MoldItemId')
BEGIN
	ALTER TABLE Mold ADD MoldItemId INT NULL
	ALTER TABLE Mold ADD CONSTRAINT FK_Mold_MoldItem FOREIGN KEY(MoldItemId) REFERENCES Item(Id)
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Mold' AND COLUMN_NAME='OwnedDate')
BEGIN
	ALTER TABLE Mold ADD OwnedDate DATETIME NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Mold' AND COLUMN_NAME='MoldStatus')
BEGIN
	ALTER TABLE Mold ADD MoldStatus INT NULL DEFAULT 1
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Mold' AND COLUMN_NAME='Explanation')
BEGIN
	ALTER TABLE Mold ADD Explanation NVARCHAR(300) NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='Invoice')
BEGIN
	CREATE TABLE [dbo].[Invoice](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceType] [int] NULL,
	[InvoiceNo] [nvarchar](50) NULL,
	[InvoiceStatus] [int] NULL,
	[DocumentNo] [nvarchar](100) NULL,
	[InvoiceDate] [datetime] NULL,
	[FirmId] [int] NULL,
	[PlantId] [int] NULL,
	[Explanation] [nvarchar](300) NULL,
	[SubTotal] [decimal](18, 5) NULL,
	[DiscountTotal] [decimal](18, 5) NULL,
	[TaxTotal] [decimal](18, 5) NULL,
	[GrandTotal] [decimal](18, 5) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedUserId] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedUserId] [int] NULL,
	 CONSTRAINT [PK_Invoice] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[Invoice]  WITH CHECK ADD  CONSTRAINT [FK_Invoice_Firm] FOREIGN KEY([FirmId])
	REFERENCES [dbo].[Firm] ([Id])

	ALTER TABLE [dbo].[Invoice] CHECK CONSTRAINT [FK_Invoice_Firm]

	ALTER TABLE [dbo].[Invoice]  WITH CHECK ADD  CONSTRAINT [FK_Invoice_Plant] FOREIGN KEY([PlantId])
	REFERENCES [dbo].[Plant] ([Id])

	ALTER TABLE [dbo].[Invoice] CHECK CONSTRAINT [FK_Invoice_Plant]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ItemReceipt' AND COLUMN_NAME='InvoiceId')
BEGIN
	ALTER TABLE ItemReceipt ADD InvoiceId INT NULL
	ALTER TABLE ItemReceipt ADD CONSTRAINT FK_ItemReceipt_Invoice FOREIGN KEY(InvoiceId) REFERENCES Invoice(Id)
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='ProductWastage')
BEGIN
	CREATE TABLE [dbo].[ProductWastage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WorkOrderDetailId] [int] NULL,
	[ProductId] [int] NULL,
	[MachineId] [int] NULL,
	[EntryDate] [datetime] NULL,
	[Quantity] [decimal](18, 5) NULL,
	[WastageStatus] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedUserId] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedUserId] [int] NULL,
	 CONSTRAINT [PK_ProductWastage] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[ProductWastage]  WITH CHECK ADD  CONSTRAINT [FK_ProductWastage_Item] FOREIGN KEY([ProductId])
	REFERENCES [dbo].[Item] ([Id])

	ALTER TABLE [dbo].[ProductWastage] CHECK CONSTRAINT [FK_ProductWastage_Item]

	ALTER TABLE [dbo].[ProductWastage]  WITH CHECK ADD  CONSTRAINT [FK_ProductWastage_Machine] FOREIGN KEY([MachineId])
	REFERENCES [dbo].[Machine] ([Id])

	ALTER TABLE [dbo].[ProductWastage] CHECK CONSTRAINT [FK_ProductWastage_Machine]

	ALTER TABLE [dbo].[ProductWastage]  WITH CHECK ADD  CONSTRAINT [FK_ProductWastage_WorkOrderDetail] FOREIGN KEY([WorkOrderDetailId])
	REFERENCES [dbo].[WorkOrderDetail] ([Id])

	ALTER TABLE [dbo].[ProductWastage] CHECK CONSTRAINT [FK_ProductWastage_WorkOrderDetail]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Machine' AND COLUMN_NAME='PostureExpirationCycleCount')
BEGIN
	ALTER TABLE Machine ADD PostureExpirationCycleCount INT NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Machine' AND COLUMN_NAME='IsUpToPostureEntry')
BEGIN
	ALTER TABLE Machine ADD IsUpToPostureEntry BIT NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Machine' AND COLUMN_NAME='WorkingUserId')
BEGIN
	ALTER TABLE Machine ADD WorkingUserId INT NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='UserWorkOrderHistory')
BEGIN
	CREATE TABLE [dbo].[UserWorkOrderHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[WorkOrderDetailId] [int] NULL,
	[MachineId] [int] NULL,
	[StartQuantity] [decimal](18, 5) NULL,
	[EndQuantity] [decimal](18, 5) NULL,
	[FinishedQuantity] [decimal](18, 5) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	 CONSTRAINT [PK_UserWorkOrderHistory] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[UserWorkOrderHistory]  WITH CHECK ADD  CONSTRAINT [FK_UserWorkOrderHistory_Machine] FOREIGN KEY([MachineId])
	REFERENCES [dbo].[Machine] ([Id])

	ALTER TABLE [dbo].[UserWorkOrderHistory] CHECK CONSTRAINT [FK_UserWorkOrderHistory_Machine]

	ALTER TABLE [dbo].[UserWorkOrderHistory]  WITH CHECK ADD  CONSTRAINT [FK_UserWorkOrderHistory_WorkOrderDetail] FOREIGN KEY([WorkOrderDetailId])
	REFERENCES [dbo].[WorkOrderDetail] ([Id])

	ALTER TABLE [dbo].[UserWorkOrderHistory] CHECK CONSTRAINT [FK_UserWorkOrderHistory_WorkOrderDetail]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='DeliveryPlan')
BEGIN
	CREATE TABLE [dbo].[DeliveryPlan](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WorkOrderDetailId] [int] NULL,
	[PlanDate] [datetime] NULL,
	[OrderNo] [int] NULL,
	[PlanStatus] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedUserId] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedUserId] [int] NULL,
	 CONSTRAINT [PK_DeliveryPlan] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[DeliveryPlan]  WITH CHECK ADD  CONSTRAINT [FK_DeliveryPlan_WorkOrderDetail] FOREIGN KEY([WorkOrderDetailId])
	REFERENCES [dbo].[WorkOrderDetail] ([Id])

	ALTER TABLE [dbo].[DeliveryPlan] CHECK CONSTRAINT [FK_DeliveryPlan_WorkOrderDetail]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='ItemOrderItemNeeds')
BEGIN
	CREATE TABLE [dbo].[ItemOrderItemNeeds](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ItemOrderDetailId] [int] NULL,
	[ItemOrderId] [int] NULL,
	[ItemId] [int] NULL,
	[Quantity] [decimal](18, 5) NULL,
	[RemainingNeedsQuantity] [decimal](18, 5) NULL,
	[CalculatedDate] [datetime] NULL,
	 CONSTRAINT [PK_ItemOrderItemNeeds] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[ItemOrderItemNeeds]  WITH CHECK ADD  CONSTRAINT [FK_ItemOrderItemNeeds_Item] FOREIGN KEY([ItemId])
	REFERENCES [dbo].[Item] ([Id])

	ALTER TABLE [dbo].[ItemOrderItemNeeds] CHECK CONSTRAINT [FK_ItemOrderItemNeeds_Item]

	ALTER TABLE [dbo].[ItemOrderItemNeeds]  WITH CHECK ADD  CONSTRAINT [FK_ItemOrderItemNeeds_ItemOrder] FOREIGN KEY([ItemOrderId])
	REFERENCES [dbo].[ItemOrder] ([Id])

	ALTER TABLE [dbo].[ItemOrderItemNeeds] CHECK CONSTRAINT [FK_ItemOrderItemNeeds_ItemOrder]

	ALTER TABLE [dbo].[ItemOrderItemNeeds]  WITH CHECK ADD  CONSTRAINT [FK_ItemOrderItemNeeds_ItemOrderDetail] FOREIGN KEY([ItemOrderDetailId])
	REFERENCES [dbo].[ItemOrderDetail] ([Id])

	ALTER TABLE [dbo].[ItemOrderItemNeeds] CHECK CONSTRAINT [FK_ItemOrderItemNeeds_ItemOrderDetail]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='WorkOrderSerial' AND COLUMN_NAME='ItemReceiptDetailId')
BEGIN
	ALTER TABLE WorkOrderSerial ADD ItemReceiptDetailId INT NULL
	ALTER TABLE WorkOrderSerial ADD CONSTRAINT FK_WorkOrderSerial_ItemReceiptDetail FOREIGN KEY(ItemReceiptDetailId) REFERENCES ItemReceiptDetail(Id)
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ProductQualityPlan' AND COLUMN_NAME='MoldTestFieldName')
BEGIN
	ALTER TABLE ProductQualityPlan ADD MoldTestFieldName NVARCHAR(100) NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Machine' AND COLUMN_NAME='BackColor')
BEGIN
	ALTER TABLE Machine ADD BackColor NVARCHAR(100) NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Machine' AND COLUMN_NAME='ForeColor')
BEGIN
	ALTER TABLE Machine ADD ForeColor NVARCHAR(100) NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='MachineSignal' AND COLUMN_NAME='ShiftId')
BEGIN
	ALTER TABLE MachineSignal ADD ShiftId INT NULL
	ALTER TABLE MachineSignal ADD CONSTRAINT FK_MachineSignal_Shift FOREIGN KEY(ShiftId) REFERENCES Shift(Id)
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='SystemPrinter')
BEGIN
	CREATE TABLE [dbo].[SystemPrinter](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PrinterCode] [nvarchar](50) NULL,
	[PrinterName] [nvarchar](250) NULL,
	[AccessPath] [nvarchar](300) NULL,
	[PlantId] [int] NULL,
	[IsActive] [bit] NULL,
	 CONSTRAINT [PK_SystemPrinter] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='SystemParameter')
BEGIN
	CREATE TABLE [dbo].[SystemParameter](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PrmCode] [nvarchar](50) NULL,
	[PrmValue] [nvarchar](300) NULL,
	[PlantId] [int] NULL,
	 CONSTRAINT [PK_SystemParameter] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='AllocatedCode')
BEGIN
	CREATE TABLE [dbo].[AllocatedCode](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AllocatedCode] [nvarchar](100) NULL,
	[ObjectType] [int] NULL,
	[CreatedDate] [datetime] NULL,
	 CONSTRAINT [PK_AllocatedCode] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	CREATE UNIQUE NONCLUSTERED INDEX [IX_AllocatedCode] ON [dbo].[AllocatedCode]
	(
		[AllocatedCode] ASC,
		[ObjectType] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='PrinterQueue')
BEGIN
	CREATE TABLE [dbo].[PrinterQueue](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PrinterId] [int] NULL,
	[RecordType] [int] NULL,
	[RecordId] [int] NULL,
	[OrderNo] [int] NULL,
	[AllocatedPrintData] [nvarchar](250) NULL,
	[CreatedDate] [datetime] NULL,
	 CONSTRAINT [PK_PrinterQueue] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[PrinterQueue]  WITH CHECK ADD  CONSTRAINT [FK_PrinterQueue_SystemPrinter] FOREIGN KEY([PrinterId])
	REFERENCES [dbo].[SystemPrinter] ([Id])

	ALTER TABLE [dbo].[PrinterQueue] CHECK CONSTRAINT [FK_PrinterQueue_SystemPrinter]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='SystemPrinter' AND COLUMN_NAME='PageWidth')
BEGIN
	ALTER TABLE SystemPrinter ADD PageWidth DECIMAL(18,5) NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='SystemPrinter' AND COLUMN_NAME='PageHeight')
BEGIN
	ALTER TABLE SystemPrinter ADD PageHeight DECIMAL(18,5) NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='ItemReceiptConsume')
BEGIN
	CREATE TABLE [dbo].[ItemReceiptConsume](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ConsumedReceiptDetailId] [int] NULL,
	[ConsumerReceiptDetailId] [int] NULL,
	[UsedQuantity] [decimal](18, 5) NULL,
	[UsedGrossQuantity] [decimal](18, 5) NULL,
	[UnitId] [int] NULL,
	 CONSTRAINT [PK_ItemReceiptConsume] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[ItemReceiptConsume]  WITH CHECK ADD  CONSTRAINT [FK_ItemReceiptConsume_ItemReceiptDetailConsumed] FOREIGN KEY([ConsumedReceiptDetailId])
	REFERENCES [dbo].[ItemReceiptDetail] ([Id])

	ALTER TABLE [dbo].[ItemReceiptConsume] CHECK CONSTRAINT [FK_ItemReceiptConsume_ItemReceiptDetailConsumed]

	ALTER TABLE [dbo].[ItemReceiptConsume]  WITH CHECK ADD  CONSTRAINT [FK_ItemReceiptConsume_ItemReceiptDetailConsumer] FOREIGN KEY([ConsumerReceiptDetailId])
	REFERENCES [dbo].[ItemReceiptDetail] ([Id])

	ALTER TABLE [dbo].[ItemReceiptConsume] CHECK CONSTRAINT [FK_ItemReceiptConsume_ItemReceiptDetailConsumer]

	ALTER TABLE [dbo].[ItemReceiptConsume]  WITH CHECK ADD  CONSTRAINT [FK_ItemReceiptConsume_UnitType] FOREIGN KEY([UnitId])
	REFERENCES [dbo].[UnitType] ([Id])

	ALTER TABLE [dbo].[ItemReceiptConsume] CHECK CONSTRAINT [FK_ItemReceiptConsume_UnitType]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='ReportTemplate')
BEGIN
	CREATE TABLE [dbo].[ReportTemplate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReportType] [int] NULL,
	[ReportCode] [nvarchar](100) NULL,
	[ReportName] [nvarchar](150) NULL,
	[FileName] [nvarchar](300) NULL,
	[IsActive] [bit] NULL,
	 CONSTRAINT [PK_ReportTemplate] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='Equipment')
BEGIN
	CREATE TABLE [dbo].[Equipment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EquipmentCode] [nvarchar](150) NULL,
	[EquipmentName] [nvarchar](250) NULL,
	[PlantId] [int] NULL,
	[MachineId] [int] NULL,
	[Manufacturer] [nvarchar](250) NULL,
	[ModelNo] [nvarchar](50) NULL,
	[SerialNo] [nvarchar](150) NULL,
	[Location] [nvarchar](150) NULL,
	[ResponsibleUserId] [int] NULL,
	 CONSTRAINT [PK_Equipment] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[Equipment]  WITH CHECK ADD  CONSTRAINT [FK_Equipment_Machine] FOREIGN KEY([MachineId])
	REFERENCES [dbo].[Machine] ([Id])

	ALTER TABLE [dbo].[Equipment] CHECK CONSTRAINT [FK_Equipment_Machine]

	ALTER TABLE [dbo].[Equipment]  WITH CHECK ADD  CONSTRAINT [FK_Equipment_Plant] FOREIGN KEY([PlantId])
	REFERENCES [dbo].[Plant] ([Id])

	ALTER TABLE [dbo].[Equipment] CHECK CONSTRAINT [FK_Equipment_Plant]

	ALTER TABLE [dbo].[Equipment]  WITH CHECK ADD  CONSTRAINT [FK_Equipment_User] FOREIGN KEY([ResponsibleUserId])
	REFERENCES [dbo].[User] ([Id])
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Equipment' AND COLUMN_NAME='IsCritical')
BEGIN
	ALTER TABLE Equipment ADD IsCritical BIT NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='EquipmentCategory')
BEGIN
	CREATE TABLE [dbo].[EquipmentCategory](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[EquipmentCategoryCode] [nvarchar](50) NULL,
		[EquipmentCategoryName] [nvarchar](150) NULL,
		[PlantId] [int] NULL,
		[IsCritical] [bit] NULL,
	 CONSTRAINT [PK_EquipmentCategory] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[EquipmentCategory]  WITH CHECK ADD  CONSTRAINT [FK_EquipmentCategory_Plant] FOREIGN KEY([PlantId])
	REFERENCES [dbo].[Plant] ([Id])

	ALTER TABLE [dbo].[EquipmentCategory] CHECK CONSTRAINT [FK_EquipmentCategory_Plant]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Equipment' AND COLUMN_NAME='EquipmentCategoryId')
BEGIN
	ALTER TABLE Equipment ADD EquipmentCategoryId INT NULL
	ALTER TABLE Equipment ADD CONSTRAINT FK_Equipment_EquipmentCategory FOREIGN KEY(EquipmentCategoryId) REFERENCES EquipmentCategory(Id)
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ProductWastage' AND COLUMN_NAME='ShiftId')
BEGIN
	ALTER TABLE ProductWastage ADD ShiftId INT NULL
	ALTER TABLE ProductWastage ADD CONSTRAINT FK_ProductWastage_Shift FOREIGN KEY(ShiftId) REFERENCES Shift(Id)
END
GO
IF NOT EXISTS(select * from UserAuthType WHERE AuthTypeCode='ModuleQuality')
BEGIN
	INSERT INTO UserAuthType(AuthTypeCode, AuthTypeName) VALUES('ModuleQuality', 'Kalite Modülü')
END
GO
IF NOT EXISTS(select * from UserAuthType WHERE AuthTypeCode='ModuleItems')
BEGIN
	INSERT INTO UserAuthType(AuthTypeCode, AuthTypeName) VALUES('ModuleItems', 'Stok Modülü')
END
GO
IF NOT EXISTS(select * from UserAuthType WHERE AuthTypeCode='ModuleProduction')
BEGIN
	INSERT INTO UserAuthType(AuthTypeCode, AuthTypeName) VALUES('ModuleProduction', 'Üretim Modülü')
END
GO
IF NOT EXISTS(select * from UserAuthType WHERE AuthTypeCode='ModuleDefinitions')
BEGIN
	INSERT INTO UserAuthType(AuthTypeCode, AuthTypeName) VALUES('ModuleDefinitions', 'Tanımlar')
END
GO
IF NOT EXISTS(select * from UserAuthType WHERE AuthTypeCode='IsSystemAdmin')
BEGIN
	INSERT INTO UserAuthType(AuthTypeCode, AuthTypeName) VALUES('IsSystemAdmin', 'Sistem Yöneticisi')
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='WorkOrderSerial' AND COLUMN_NAME='QualityStatus')
BEGIN
	ALTER TABLE WorkOrderSerial ADD QualityStatus INT NULL
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='WorkOrderDetail' AND COLUMN_NAME='QualityStatus')
BEGIN
	ALTER TABLE WorkOrderDetail ADD QualityStatus INT NULL
END