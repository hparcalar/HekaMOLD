IF NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Item' AND COLUMN_NAME='MoldId')
BEGIN
	ALTER TABLE Item ADD MoldId INT NULL
	ALTER TABLE Item ADD CONSTRAINT FK_Item_Mold FOREIGN KEY(MoldId) REFERENCES Mold(Id)
END
GO
IF NOT EXISTS(select * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='WorkOrderItemNeeds')
BEGIN
	CREATE TABLE [dbo].[WorkOrderItemNeeds](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[WorkOrderDetailId] [int] NULL,
		[WorkOrderId] [int] NULL,
		[ItemId] [int] NULL,
		[Quantity] [decimal](18, 5) NULL,
		[RemainingNeedsQuantity] [decimal](18, 5) NULL,
		[CalculatedDate] [datetime] NULL,
	 CONSTRAINT [PK_WorkOrderItemNeeds] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[WorkOrderItemNeeds]  WITH CHECK ADD  CONSTRAINT [FK_WorkOrderItemNeeds_Item] FOREIGN KEY([ItemId])
	REFERENCES [dbo].[Item] ([Id])

	ALTER TABLE [dbo].[WorkOrderItemNeeds] CHECK CONSTRAINT [FK_WorkOrderItemNeeds_Item]

	ALTER TABLE [dbo].[WorkOrderItemNeeds]  WITH CHECK ADD  CONSTRAINT [FK_WorkOrderItemNeeds_WorkOrder] FOREIGN KEY([WorkOrderId])
	REFERENCES [dbo].[WorkOrder] ([Id])

	ALTER TABLE [dbo].[WorkOrderItemNeeds] CHECK CONSTRAINT [FK_WorkOrderItemNeeds_WorkOrder]

	ALTER TABLE [dbo].[WorkOrderItemNeeds]  WITH CHECK ADD  CONSTRAINT [FK_WorkOrderItemNeeds_WorkOrderDetail] FOREIGN KEY([WorkOrderDetailId])
	REFERENCES [dbo].[WorkOrderDetail] ([Id])

	ALTER TABLE [dbo].[WorkOrderItemNeeds] CHECK CONSTRAINT [FK_WorkOrderItemNeeds_WorkOrderDetail]
END