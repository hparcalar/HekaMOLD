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