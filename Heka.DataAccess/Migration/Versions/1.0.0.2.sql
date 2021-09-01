IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='ProductionPosture')
BEGIN
	CREATE TABLE [dbo].[ProductionPosture](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MachineId] [int] NULL,
	[WorkOrderDetailId] [int] NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[PostureStatus] [int] NULL,
	[Reason] [nvarchar](300) NULL,
	[Explanation] [nvarchar](400) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedUserId] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedUserId] [int] NULL,
	 CONSTRAINT [PK_ProductionPosture] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[ProductionPosture]  WITH CHECK ADD  CONSTRAINT [FK_ProductionPosture_Machine] FOREIGN KEY([MachineId])
	REFERENCES [dbo].[Machine] ([Id])

	ALTER TABLE [dbo].[ProductionPosture] CHECK CONSTRAINT [FK_ProductionPosture_Machine]

	ALTER TABLE [dbo].[ProductionPosture]  WITH CHECK ADD  CONSTRAINT [FK_ProductionPosture_WorkOrderDetail] FOREIGN KEY([WorkOrderDetailId])
	REFERENCES [dbo].[WorkOrderDetail] ([Id])

	ALTER TABLE [dbo].[ProductionPosture] CHECK CONSTRAINT [FK_ProductionPosture_WorkOrderDetail]
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='MachineSignal')
BEGIN
CREATE TABLE [dbo].[MachineSignal](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MachineId] [int] NULL,
	[WorkOrderDetailId] [int] NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Duration] [int] NULL,
	[SignalStatus] [int] NULL,
	 CONSTRAINT [PK_MachineSignal] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[MachineSignal]  WITH CHECK ADD  CONSTRAINT [FK_MachineSignal_Machine] FOREIGN KEY([MachineId])
	REFERENCES [dbo].[Machine] ([Id])

	ALTER TABLE [dbo].[MachineSignal] CHECK CONSTRAINT [FK_MachineSignal_Machine]

	ALTER TABLE [dbo].[MachineSignal]  WITH CHECK ADD  CONSTRAINT [FK_MachineSignal_WorkOrderDetail] FOREIGN KEY([WorkOrderDetailId])
	REFERENCES [dbo].[WorkOrderDetail] ([Id])

	ALTER TABLE [dbo].[MachineSignal] CHECK CONSTRAINT [FK_MachineSignal_WorkOrderDetail]
END