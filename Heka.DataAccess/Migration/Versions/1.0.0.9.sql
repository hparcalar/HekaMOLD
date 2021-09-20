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
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
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
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
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
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
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