namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CodeFirstInitial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActualRouteHistory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WorkOrderDetailId = c.Int(),
                        ProcessId = c.Int(),
                        ProcessGroupId = c.Int(),
                        MachineId = c.Int(),
                        MachineGroupId = c.Int(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        ProcessStatus = c.Int(),
                        StartUserId = c.Int(),
                        EndUserId = c.Int(),
                        Explanation = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Machine", t => t.MachineId)
                .ForeignKey("dbo.WorkOrderDetail", t => t.WorkOrderDetailId)
                .ForeignKey("dbo.MachineGroup", t => t.MachineGroupId)
                .ForeignKey("dbo.Process", t => t.ProcessId)
                .ForeignKey("dbo.ProcessGroup", t => t.ProcessGroupId)
                .Index(t => t.WorkOrderDetailId)
                .Index(t => t.ProcessId)
                .Index(t => t.ProcessGroupId)
                .Index(t => t.MachineId)
                .Index(t => t.MachineGroupId);
            
            CreateTable(
                "dbo.Machine",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MachineCode = c.String(),
                        MachineName = c.String(),
                        MachineType = c.Int(),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                        IsActive = c.Boolean(),
                        IsWatched = c.Boolean(),
                        WatchCycleStartCondition = c.String(),
                        DeviceIp = c.String(),
                        PostureExpirationCycleCount = c.Int(),
                        IsUpToPostureEntry = c.Boolean(),
                        WorkingUserId = c.Int(),
                        BackColor = c.String(),
                        ForeColor = c.String(),
                        MachineStatus = c.Int(),
                        MachineGroupId = c.Int(),
                        SignalEndDelay = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MachineGroup", t => t.MachineGroupId)
                .Index(t => t.MachineGroupId);
            
            CreateTable(
                "dbo.Equipment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EquipmentCode = c.String(),
                        EquipmentName = c.String(),
                        PlantId = c.Int(),
                        MachineId = c.Int(),
                        Manufacturer = c.String(),
                        ModelNo = c.String(),
                        SerialNo = c.String(),
                        Location = c.String(),
                        ResponsibleUserId = c.Int(),
                        IsCritical = c.Boolean(),
                        EquipmentCategoryId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EquipmentCategory", t => t.EquipmentCategoryId)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .ForeignKey("dbo.User", t => t.ResponsibleUserId)
                .ForeignKey("dbo.Machine", t => t.MachineId)
                .Index(t => t.PlantId)
                .Index(t => t.MachineId)
                .Index(t => t.ResponsibleUserId)
                .Index(t => t.EquipmentCategoryId);
            
            CreateTable(
                "dbo.EquipmentCategory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EquipmentCategoryCode = c.String(),
                        EquipmentCategoryName = c.String(),
                        PlantId = c.Int(),
                        IsCritical = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.PlantId);
            
            CreateTable(
                "dbo.Plant",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PlantCode = c.String(),
                        PlantName = c.String(),
                        LogoData = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Firm",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirmCode = c.String(),
                        FirmName = c.String(),
                        FirmTitle = c.String(),
                        PlantId = c.Int(),
                        FirmType = c.Int(),
                        Explanation = c.String(),
                        Phone = c.String(),
                        Gsm = c.String(),
                        Facebook = c.String(),
                        Twitter = c.String(),
                        Instagram = c.String(),
                        Address = c.String(),
                        Address2 = c.String(),
                        Author = c.String(),
                        Email = c.String(),
                        TaxNo = c.String(),
                        TaxOffice = c.String(),
                        IsApproved = c.Boolean(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.PlantId);
            
            CreateTable(
                "dbo.EntryQualityData",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QualityPlanId = c.Int(),
                        QualityPlanDetailId = c.Int(),
                        IsOk = c.Boolean(),
                        Explanation = c.String(),
                        ItemEntryDetailId = c.Int(),
                        ItemLot = c.String(),
                        ItemNo = c.String(),
                        ItemName = c.String(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                        FirmId = c.Int(),
                        ItemId = c.Int(),
                        WaybillNo = c.String(),
                        EntryQuantity = c.Decimal(precision: 18, scale: 2),
                        CheckedQuantity = c.Decimal(precision: 18, scale: 2),
                        LotNumbers = c.String(),
                        SampleQuantity = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EntryQualityPlanDetail", t => t.QualityPlanDetailId)
                .ForeignKey("dbo.Item", t => t.ItemId)
                .ForeignKey("dbo.ItemReceiptDetail", t => t.ItemEntryDetailId)
                .ForeignKey("dbo.EntryQualityPlan", t => t.QualityPlanId)
                .ForeignKey("dbo.Firm", t => t.FirmId)
                .Index(t => t.QualityPlanId)
                .Index(t => t.QualityPlanDetailId)
                .Index(t => t.ItemEntryDetailId)
                .Index(t => t.FirmId)
                .Index(t => t.ItemId);
            
            CreateTable(
                "dbo.EntryQualityDataDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EntryQualityDataId = c.Int(),
                        EntryQualityPlanDetailId = c.Int(),
                        OrderNo = c.Int(),
                        FaultExplanation = c.String(),
                        SampleQuantity = c.Decimal(precision: 18, scale: 2),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EntryQualityPlanDetail", t => t.EntryQualityPlanDetailId)
                .ForeignKey("dbo.EntryQualityData", t => t.EntryQualityDataId)
                .Index(t => t.EntryQualityDataId)
                .Index(t => t.EntryQualityPlanDetailId);
            
            CreateTable(
                "dbo.EntryQualityPlanDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EntryQualityPlanId = c.Int(),
                        CheckProperty = c.String(),
                        IsRequired = c.Boolean(),
                        OrderNo = c.Int(),
                        PeriodType = c.String(),
                        AcceptanceCriteria = c.String(),
                        ControlDevice = c.String(),
                        Method = c.String(),
                        Responsible = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EntryQualityPlan", t => t.EntryQualityPlanId)
                .Index(t => t.EntryQualityPlanId);
            
            CreateTable(
                "dbo.EntryQualityPlan",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QualityControlCode = c.String(),
                        OrderNo = c.Int(),
                        ItemGroupId = c.Int(),
                        ItemCategoryId = c.Int(),
                        ItemGroupText = c.String(),
                        PeriodType = c.String(),
                        AcceptanceCriteria = c.String(),
                        ControlDevice = c.String(),
                        Method = c.String(),
                        Responsible = c.String(),
                        RecordType = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ItemCategory", t => t.ItemCategoryId)
                .ForeignKey("dbo.ItemGroup", t => t.ItemGroupId)
                .Index(t => t.ItemGroupId)
                .Index(t => t.ItemCategoryId);
            
            CreateTable(
                "dbo.ItemCategory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemCategoryCode = c.String(),
                        ItemCategoryName = c.String(),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.PlantId);
            
            CreateTable(
                "dbo.Item",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemNo = c.String(),
                        ItemName = c.String(),
                        ItemType = c.Int(),
                        ItemCategoryId = c.Int(),
                        ItemGroupId = c.Int(),
                        SupplierFirmId = c.Int(),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                        MoldId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ItemGroup", t => t.ItemGroupId)
                .ForeignKey("dbo.Mold", t => t.MoldId)
                .ForeignKey("dbo.ItemCategory", t => t.ItemCategoryId)
                .ForeignKey("dbo.Firm", t => t.SupplierFirmId)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.ItemCategoryId)
                .Index(t => t.ItemGroupId)
                .Index(t => t.SupplierFirmId)
                .Index(t => t.PlantId)
                .Index(t => t.MoldId);
            
            CreateTable(
                "dbo.ItemGroup",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemGroupCode = c.String(),
                        ItemGroupName = c.String(),
                        ItemCategoryId = c.Int(),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ItemCategory", t => t.ItemCategoryId)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.ItemCategoryId)
                .Index(t => t.PlantId);
            
            CreateTable(
                "dbo.ItemLiveStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemId = c.Int(),
                        WarehouseId = c.Int(),
                        InQuantity = c.Decimal(precision: 18, scale: 2),
                        OutQuantity = c.Decimal(precision: 18, scale: 2),
                        LiveQuantity = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Warehouse", t => t.WarehouseId)
                .ForeignKey("dbo.Item", t => t.ItemId)
                .Index(t => t.ItemId)
                .Index(t => t.WarehouseId);
            
            CreateTable(
                "dbo.Warehouse",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WarehouseCode = c.String(),
                        WarehouseName = c.String(),
                        WarehouseType = c.Int(),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.PlantId);
            
            CreateTable(
                "dbo.ItemOrderDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemOrderId = c.Int(),
                        LineNumber = c.Int(),
                        ItemId = c.Int(),
                        UnitId = c.Int(),
                        Quantity = c.Decimal(precision: 18, scale: 2),
                        NetQuantity = c.Decimal(precision: 18, scale: 2),
                        GrossQuantity = c.Decimal(precision: 18, scale: 2),
                        UnitPrice = c.Decimal(precision: 18, scale: 2),
                        ForexId = c.Int(),
                        ForexRate = c.Decimal(precision: 18, scale: 2),
                        ForexUnitPrice = c.Decimal(precision: 18, scale: 2),
                        TaxIncluded = c.Boolean(),
                        TaxRate = c.Int(),
                        TaxAmount = c.Decimal(precision: 18, scale: 2),
                        DiscountRate = c.Decimal(precision: 18, scale: 2),
                        DiscountAmount = c.Decimal(precision: 18, scale: 2),
                        SubTotal = c.Decimal(precision: 18, scale: 2),
                        OverallTotal = c.Decimal(precision: 18, scale: 2),
                        Explanation = c.String(),
                        OrderStatus = c.Int(),
                        SyncStatus = c.Int(),
                        SyncDate = c.DateTime(),
                        ItemRequestDetailId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ForexType", t => t.ForexId)
                .ForeignKey("dbo.UnitType", t => t.UnitId)
                .ForeignKey("dbo.ItemRequestDetail", t => t.ItemRequestDetailId)
                .ForeignKey("dbo.ItemOrder", t => t.ItemOrderId)
                .ForeignKey("dbo.Item", t => t.ItemId)
                .Index(t => t.ItemOrderId)
                .Index(t => t.ItemId)
                .Index(t => t.UnitId)
                .Index(t => t.ForexId)
                .Index(t => t.ItemRequestDetailId);
            
            CreateTable(
                "dbo.ForexType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ForexTypeCode = c.String(),
                        ActiveSalesRate = c.Decimal(precision: 18, scale: 2),
                        ActiveBuyingRate = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ForexHistory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ForexId = c.Int(),
                        HistoryDate = c.DateTime(),
                        SalesForexRate = c.Decimal(precision: 18, scale: 2),
                        BuyForexRate = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ForexType", t => t.ForexId)
                .Index(t => t.ForexId);
            
            CreateTable(
                "dbo.ItemReceiptDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemReceiptId = c.Int(),
                        LineNumber = c.Int(),
                        ItemId = c.Int(),
                        UnitId = c.Int(),
                        Quantity = c.Decimal(precision: 18, scale: 2),
                        NetQuantity = c.Decimal(precision: 18, scale: 2),
                        GrossQuantity = c.Decimal(precision: 18, scale: 2),
                        UsableQuantity = c.Decimal(precision: 18, scale: 2),
                        UnitPrice = c.Decimal(precision: 18, scale: 2),
                        ForexId = c.Int(),
                        ForexRate = c.Decimal(precision: 18, scale: 2),
                        ForexUnitPrice = c.Decimal(precision: 18, scale: 2),
                        TaxIncluded = c.Boolean(),
                        TaxRate = c.Int(),
                        TaxAmount = c.Decimal(precision: 18, scale: 2),
                        DiscountRate = c.Decimal(precision: 18, scale: 2),
                        DiscountAmount = c.Decimal(precision: 18, scale: 2),
                        SubTotal = c.Decimal(precision: 18, scale: 2),
                        OverallTotal = c.Decimal(precision: 18, scale: 2),
                        Explanation = c.String(),
                        ReceiptStatus = c.Int(),
                        SyncStatus = c.Int(),
                        SyncDate = c.DateTime(),
                        ItemOrderDetailId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UnitType", t => t.UnitId)
                .ForeignKey("dbo.ItemReceipt", t => t.ItemReceiptId)
                .ForeignKey("dbo.ForexType", t => t.ForexId)
                .ForeignKey("dbo.ItemOrderDetail", t => t.ItemOrderDetailId)
                .ForeignKey("dbo.Item", t => t.ItemId)
                .Index(t => t.ItemReceiptId)
                .Index(t => t.ItemId)
                .Index(t => t.UnitId)
                .Index(t => t.ForexId)
                .Index(t => t.ItemOrderDetailId);
            
            CreateTable(
                "dbo.ItemOrderConsume",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemOrderDetailId = c.Int(),
                        ConsumerReceiptDetailId = c.Int(),
                        ConsumedReceiptDetailId = c.Int(),
                        UsedQuantity = c.Decimal(precision: 18, scale: 2),
                        UsedGrossQuantity = c.Decimal(precision: 18, scale: 2),
                        UnitId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UnitType", t => t.UnitId)
                .ForeignKey("dbo.ItemReceiptDetail", t => t.ConsumedReceiptDetailId)
                .ForeignKey("dbo.ItemReceiptDetail", t => t.ConsumerReceiptDetailId)
                .ForeignKey("dbo.ItemOrderDetail", t => t.ItemOrderDetailId)
                .Index(t => t.ItemOrderDetailId)
                .Index(t => t.ConsumerReceiptDetailId)
                .Index(t => t.ConsumedReceiptDetailId)
                .Index(t => t.UnitId);
            
            CreateTable(
                "dbo.UnitType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UnitCode = c.String(),
                        UnitName = c.String(),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.PlantId);
            
            CreateTable(
                "dbo.ItemReceiptConsume",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ConsumedReceiptDetailId = c.Int(),
                        ConsumerReceiptDetailId = c.Int(),
                        UsedQuantity = c.Decimal(precision: 18, scale: 2),
                        UsedGrossQuantity = c.Decimal(precision: 18, scale: 2),
                        UnitId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UnitType", t => t.UnitId)
                .ForeignKey("dbo.ItemReceiptDetail", t => t.ConsumedReceiptDetailId)
                .ForeignKey("dbo.ItemReceiptDetail", t => t.ConsumerReceiptDetailId)
                .Index(t => t.ConsumedReceiptDetailId)
                .Index(t => t.ConsumerReceiptDetailId)
                .Index(t => t.UnitId);
            
            CreateTable(
                "dbo.ItemRequestDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemRequestId = c.Int(),
                        LineNumber = c.Int(),
                        ItemId = c.Int(),
                        UnitId = c.Int(),
                        Quantity = c.Decimal(precision: 18, scale: 2),
                        NetQuantity = c.Decimal(precision: 18, scale: 2),
                        ApprovedQuantity = c.Decimal(precision: 18, scale: 2),
                        Explanation = c.String(),
                        RequestStatus = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ItemRequest", t => t.ItemRequestId)
                .ForeignKey("dbo.UnitType", t => t.UnitId)
                .ForeignKey("dbo.Item", t => t.ItemId)
                .Index(t => t.ItemRequestId)
                .Index(t => t.ItemId)
                .Index(t => t.UnitId);
            
            CreateTable(
                "dbo.ItemRequest",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RequestNo = c.String(),
                        RequestType = c.Int(),
                        RequestCategoryId = c.Int(),
                        DateOfNeed = c.DateTime(),
                        RequestStatus = c.Int(),
                        PlantId = c.Int(),
                        Explanation = c.String(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.CreatedUserId)
                .ForeignKey("dbo.ItemRequestCategory", t => t.RequestCategoryId)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.RequestCategoryId)
                .Index(t => t.PlantId)
                .Index(t => t.CreatedUserId);
            
            CreateTable(
                "dbo.ItemOrder",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderNo = c.String(),
                        DocumentNo = c.String(),
                        OrderDate = c.DateTime(),
                        OrderType = c.Int(),
                        DateOfNeed = c.DateTime(),
                        FirmId = c.Int(),
                        InWarehouseId = c.Int(),
                        OutWarehouseId = c.Int(),
                        PlantId = c.Int(),
                        Explanation = c.String(),
                        OrderStatus = c.Int(),
                        ItemRequestId = c.Int(),
                        SubTotal = c.Decimal(precision: 18, scale: 2),
                        TaxPrice = c.Decimal(precision: 18, scale: 2),
                        OverallTotal = c.Decimal(precision: 18, scale: 2),
                        SyncStatus = c.Int(),
                        SyncPointId = c.Int(),
                        SyncDate = c.DateTime(),
                        SyncUserId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                        SyncKey = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SyncPoint", t => t.SyncPointId)
                .ForeignKey("dbo.ItemRequest", t => t.ItemRequestId)
                .ForeignKey("dbo.Warehouse", t => t.InWarehouseId)
                .ForeignKey("dbo.Warehouse", t => t.OutWarehouseId)
                .ForeignKey("dbo.Firm", t => t.FirmId)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.FirmId)
                .Index(t => t.InWarehouseId)
                .Index(t => t.OutWarehouseId)
                .Index(t => t.PlantId)
                .Index(t => t.ItemRequestId)
                .Index(t => t.SyncPointId);
            
            CreateTable(
                "dbo.ItemOrderItemNeeds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemOrderDetailId = c.Int(),
                        ItemOrderId = c.Int(),
                        ItemId = c.Int(),
                        Quantity = c.Decimal(precision: 18, scale: 2),
                        RemainingNeedsQuantity = c.Decimal(precision: 18, scale: 2),
                        CalculatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ItemOrder", t => t.ItemOrderId)
                .ForeignKey("dbo.ItemOrderDetail", t => t.ItemOrderDetailId)
                .ForeignKey("dbo.Item", t => t.ItemId)
                .Index(t => t.ItemOrderDetailId)
                .Index(t => t.ItemOrderId)
                .Index(t => t.ItemId);
            
            CreateTable(
                "dbo.ItemReceipt",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReceiptNo = c.String(),
                        DocumentNo = c.String(),
                        ReceiptDate = c.DateTime(),
                        ReceiptType = c.Int(),
                        FirmId = c.Int(),
                        InWarehouseId = c.Int(),
                        OutWarehouseId = c.Int(),
                        PlantId = c.Int(),
                        DeliveryAddress = c.String(),
                        Explanation = c.String(),
                        ReceiptStatus = c.Int(),
                        SyncStatus = c.Int(),
                        SyncPointId = c.Int(),
                        SyncDate = c.DateTime(),
                        SyncUserId = c.Int(),
                        ItemOrderId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                        InvoiceId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Invoice", t => t.InvoiceId)
                .ForeignKey("dbo.SyncPoint", t => t.SyncPointId)
                .ForeignKey("dbo.Warehouse", t => t.InWarehouseId)
                .ForeignKey("dbo.Warehouse", t => t.OutWarehouseId)
                .ForeignKey("dbo.ItemOrder", t => t.ItemOrderId)
                .ForeignKey("dbo.Firm", t => t.FirmId)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.FirmId)
                .Index(t => t.InWarehouseId)
                .Index(t => t.OutWarehouseId)
                .Index(t => t.PlantId)
                .Index(t => t.SyncPointId)
                .Index(t => t.ItemOrderId)
                .Index(t => t.InvoiceId);
            
            CreateTable(
                "dbo.Invoice",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InvoiceType = c.Int(),
                        InvoiceNo = c.String(),
                        InvoiceStatus = c.Int(),
                        DocumentNo = c.String(),
                        InvoiceDate = c.DateTime(),
                        FirmId = c.Int(),
                        PlantId = c.Int(),
                        Explanation = c.String(),
                        SubTotal = c.Decimal(precision: 18, scale: 2),
                        DiscountTotal = c.Decimal(precision: 18, scale: 2),
                        TaxTotal = c.Decimal(precision: 18, scale: 2),
                        GrandTotal = c.Decimal(precision: 18, scale: 2),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Firm", t => t.FirmId)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.FirmId)
                .Index(t => t.PlantId);
            
            CreateTable(
                "dbo.SyncPoint",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SyncPointCode = c.String(),
                        SyncPointName = c.String(),
                        SyncPointType = c.Int(),
                        ConnectionString = c.String(),
                        PlantId = c.Int(),
                        IsActive = c.Boolean(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                        EnabledOnPurchaseOrders = c.Boolean(),
                        EnabledOnSalesOrders = c.Boolean(),
                        EnabledOnPurchaseItemReceipts = c.Boolean(),
                        EnabledOnSalesItemReceipts = c.Boolean(),
                        EnabledOnConsumptionReceipts = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.PlantId);
            
            CreateTable(
                "dbo.ItemRequestApproveLog",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemRequestId = c.Int(),
                        OldRequestStatus = c.Int(),
                        NewRequestStatus = c.Int(),
                        ActorUserId = c.Int(),
                        CreatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.ActorUserId)
                .ForeignKey("dbo.ItemRequest", t => t.ItemRequestId)
                .Index(t => t.ItemRequestId)
                .Index(t => t.ActorUserId);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserCode = c.String(),
                        UserName = c.String(),
                        Login = c.String(),
                        Password = c.String(),
                        PlantId = c.Int(),
                        UserRoleId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                        ProfileImage = c.Binary(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserRole", t => t.UserRoleId)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.PlantId)
                .Index(t => t.UserRoleId);
            
            CreateTable(
                "dbo.Notification",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(),
                        NotifyType = c.Int(),
                        Title = c.String(),
                        Message = c.String(),
                        RecordId = c.Int(),
                        SeenStatus = c.Int(),
                        SeenDate = c.DateTime(),
                        IsProcessed = c.Boolean(),
                        ProcessedDate = c.DateTime(),
                        CreatedDate = c.DateTime(),
                        PushStatus = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Shift",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShiftCode = c.String(),
                        ShiftName = c.String(),
                        StartTime = c.Time(precision: 7),
                        EndTime = c.Time(precision: 7),
                        IsActive = c.Boolean(),
                        ShiftChiefId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.ShiftChiefId)
                .Index(t => t.ShiftChiefId);
            
            CreateTable(
                "dbo.Incident",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MachineId = c.Int(),
                        IncidentCategoryId = c.Int(),
                        IncidentStatus = c.Int(),
                        Description = c.String(),
                        CreatedDate = c.DateTime(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        StartedUserId = c.Int(),
                        EndUserId = c.Int(),
                        ShiftId = c.Int(),
                        ShiftBelongsToDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.IncidentCategory", t => t.IncidentCategoryId)
                .ForeignKey("dbo.Shift", t => t.ShiftId)
                .ForeignKey("dbo.Machine", t => t.MachineId)
                .Index(t => t.MachineId)
                .Index(t => t.IncidentCategoryId)
                .Index(t => t.ShiftId);
            
            CreateTable(
                "dbo.IncidentCategory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IncidentCategoryCode = c.String(),
                        IncidentCategoryName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MachineSignal",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MachineId = c.Int(),
                        WorkOrderDetailId = c.Int(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        Duration = c.Int(),
                        SignalStatus = c.Int(),
                        ShiftId = c.Int(),
                        ShiftBelongsToDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkOrderDetail", t => t.WorkOrderDetailId)
                .ForeignKey("dbo.Shift", t => t.ShiftId)
                .ForeignKey("dbo.Machine", t => t.MachineId)
                .Index(t => t.MachineId)
                .Index(t => t.WorkOrderDetailId)
                .Index(t => t.ShiftId);
            
            CreateTable(
                "dbo.WorkOrderDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WorkOrderId = c.Int(),
                        ItemId = c.Int(),
                        MoldId = c.Int(),
                        DyeId = c.Int(),
                        MachineId = c.Int(),
                        MoldTestId = c.Int(),
                        InflationTimeSeconds = c.Int(),
                        RawGr = c.Decimal(precision: 18, scale: 2),
                        RawGrToleration = c.Decimal(precision: 18, scale: 2),
                        Quantity = c.Decimal(precision: 18, scale: 2),
                        WorkOrderStatus = c.Int(),
                        InPalletPackageQuantity = c.Int(),
                        InPackageQuantity = c.Int(),
                        SaleOrderDetailId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                        QualityStatus = c.Int(),
                        WorkOrderType = c.Int(),
                        TrialProductName = c.String(),
                        LabelConfig = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Mold", t => t.MoldId)
                .ForeignKey("dbo.MoldTest", t => t.MoldTestId)
                .ForeignKey("dbo.Dye", t => t.DyeId)
                .ForeignKey("dbo.WorkOrder", t => t.WorkOrderId)
                .ForeignKey("dbo.ItemOrderDetail", t => t.SaleOrderDetailId)
                .ForeignKey("dbo.Item", t => t.ItemId)
                .ForeignKey("dbo.Machine", t => t.MachineId)
                .Index(t => t.WorkOrderId)
                .Index(t => t.ItemId)
                .Index(t => t.MoldId)
                .Index(t => t.DyeId)
                .Index(t => t.MachineId)
                .Index(t => t.MoldTestId)
                .Index(t => t.SaleOrderDetailId);
            
            CreateTable(
                "dbo.DeliveryPlan",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WorkOrderDetailId = c.Int(),
                        PlanDate = c.DateTime(),
                        OrderNo = c.Int(),
                        PlanStatus = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkOrderDetail", t => t.WorkOrderDetailId)
                .Index(t => t.WorkOrderDetailId);
            
            CreateTable(
                "dbo.Dye",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DyeCode = c.String(),
                        DyeName = c.String(),
                        RalCode = c.String(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                        IsActive = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MoldTest",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MachineId = c.Int(),
                        TestDate = c.DateTime(),
                        ProductDescription = c.String(),
                        RawMaterialId = c.Int(),
                        RawMaterialName = c.String(),
                        RawMaterialGr = c.Decimal(precision: 18, scale: 2),
                        RawMaterialGrText = c.String(),
                        RawMaterialTolerationGr = c.Decimal(precision: 18, scale: 2),
                        InflationTimeSeconds = c.Int(),
                        TotalTimeSeconds = c.Int(),
                        DyeId = c.Int(),
                        DyeCode = c.String(),
                        RalCode = c.String(),
                        PackageDimension = c.String(),
                        InPackageQuantity = c.Int(),
                        NutQuantity = c.Int(),
                        NutCaliber = c.String(),
                        InPalletPackageQuantity = c.Int(),
                        ProductId = c.Int(),
                        ProductCode = c.String(),
                        ProductName = c.String(),
                        MoldId = c.Int(),
                        MoldCode = c.String(),
                        MoldName = c.String(),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                        HeadSize = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Mold", t => t.MoldId)
                .ForeignKey("dbo.Dye", t => t.DyeId)
                .ForeignKey("dbo.Machine", t => t.MachineId)
                .Index(t => t.MachineId)
                .Index(t => t.DyeId)
                .Index(t => t.MoldId);
            
            CreateTable(
                "dbo.Mold",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MoldCode = c.String(),
                        MoldName = c.String(),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                        IsActive = c.Boolean(),
                        FirmId = c.Int(),
                        LifeTimeTicks = c.Int(),
                        CurrentTicks = c.Int(),
                        MoldItemId = c.Int(),
                        OwnedDate = c.DateTime(),
                        MoldStatus = c.Int(),
                        Explanation = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Item", t => t.MoldItemId)
                .ForeignKey("dbo.Firm", t => t.FirmId)
                .Index(t => t.FirmId)
                .Index(t => t.MoldItemId);
            
            CreateTable(
                "dbo.MoldProduct",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MoldId = c.Int(),
                        ProductId = c.Int(),
                        LineNumber = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Mold", t => t.MoldId)
                .ForeignKey("dbo.Item", t => t.ProductId)
                .Index(t => t.MoldId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.ItemSerial",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemId = c.Int(),
                        ItemReceiptDetailId = c.Int(),
                        SerialNo = c.String(),
                        ReceiptType = c.Int(),
                        SerialStatus = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                        FirstQuantity = c.Decimal(precision: 18, scale: 2),
                        LiveQuantity = c.Decimal(precision: 18, scale: 2),
                        WorkOrderDetailId = c.Int(),
                        SerialType = c.Int(),
                        InPackageQuantity = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkOrderDetail", t => t.WorkOrderDetailId)
                .ForeignKey("dbo.ItemReceiptDetail", t => t.ItemReceiptDetailId)
                .ForeignKey("dbo.Item", t => t.ItemId)
                .Index(t => t.ItemId)
                .Index(t => t.ItemReceiptDetailId)
                .Index(t => t.WorkOrderDetailId);
            
            CreateTable(
                "dbo.MachinePlan",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MachineId = c.Int(),
                        WorkOrderDetailId = c.Int(),
                        OrderNo = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkOrderDetail", t => t.WorkOrderDetailId)
                .ForeignKey("dbo.Machine", t => t.MachineId)
                .Index(t => t.MachineId)
                .Index(t => t.WorkOrderDetailId);
            
            CreateTable(
                "dbo.ProductionPosture",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MachineId = c.Int(),
                        WorkOrderDetailId = c.Int(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        PostureStatus = c.Int(),
                        Reason = c.String(),
                        Explanation = c.String(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                        PostureCategoryId = c.Int(),
                        ShiftId = c.Int(),
                        ShiftBelongsToDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PostureCategory", t => t.PostureCategoryId)
                .ForeignKey("dbo.WorkOrderDetail", t => t.WorkOrderDetailId)
                .ForeignKey("dbo.Shift", t => t.ShiftId)
                .ForeignKey("dbo.Machine", t => t.MachineId)
                .Index(t => t.MachineId)
                .Index(t => t.WorkOrderDetailId)
                .Index(t => t.PostureCategoryId)
                .Index(t => t.ShiftId);
            
            CreateTable(
                "dbo.PostureCategory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PostureCategoryCode = c.String(),
                        PostureCategoryName = c.String(),
                        ShouldStopSignal = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductWastage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WorkOrderDetailId = c.Int(),
                        ProductId = c.Int(),
                        MachineId = c.Int(),
                        EntryDate = c.DateTime(),
                        Quantity = c.Decimal(precision: 18, scale: 2),
                        WastageStatus = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                        ShiftId = c.Int(),
                        ShiftBelongsToDate = c.DateTime(),
                        IsAfterScrap = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkOrderDetail", t => t.WorkOrderDetailId)
                .ForeignKey("dbo.Shift", t => t.ShiftId)
                .ForeignKey("dbo.Item", t => t.ProductId)
                .ForeignKey("dbo.Machine", t => t.MachineId)
                .Index(t => t.WorkOrderDetailId)
                .Index(t => t.ProductId)
                .Index(t => t.MachineId)
                .Index(t => t.ShiftId);
            
            CreateTable(
                "dbo.UserWorkOrderHistory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(),
                        WorkOrderDetailId = c.Int(),
                        MachineId = c.Int(),
                        StartQuantity = c.Decimal(precision: 18, scale: 2),
                        EndQuantity = c.Decimal(precision: 18, scale: 2),
                        FinishedQuantity = c.Decimal(precision: 18, scale: 2),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkOrderDetail", t => t.WorkOrderDetailId)
                .ForeignKey("dbo.Machine", t => t.MachineId)
                .Index(t => t.WorkOrderDetailId)
                .Index(t => t.MachineId);
            
            CreateTable(
                "dbo.WorkOrder",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WorkOrderNo = c.String(),
                        DocumentNo = c.String(),
                        WorkOrderDate = c.DateTime(),
                        FirmId = c.Int(),
                        PlantId = c.Int(),
                        WorkOrderStatus = c.Int(),
                        Explanation = c.String(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                        WorkOrderType = c.Int(),
                        TrialFirmName = c.String(),
                        WorkOrderCategoryId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkOrderCategory", t => t.WorkOrderCategoryId)
                .ForeignKey("dbo.Firm", t => t.FirmId)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.FirmId)
                .Index(t => t.PlantId)
                .Index(t => t.WorkOrderCategoryId);
            
            CreateTable(
                "dbo.WorkOrderCategory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WorkOrderCategoryCode = c.String(),
                        WorkOrderCategoryName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WorkOrderControl",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WorkOrderId = c.Int(),
                        WorkOrderDetailId = c.Int(),
                        ControlTypeId = c.Int(),
                        ControlValue = c.String(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkOrderControlType", t => t.ControlTypeId)
                .ForeignKey("dbo.WorkOrder", t => t.WorkOrderId)
                .ForeignKey("dbo.WorkOrderDetail", t => t.WorkOrderDetailId)
                .Index(t => t.WorkOrderId)
                .Index(t => t.WorkOrderDetailId)
                .Index(t => t.ControlTypeId);
            
            CreateTable(
                "dbo.WorkOrderControlType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ControlCode = c.String(),
                        ControlName = c.String(),
                        OrderNo = c.Int(),
                        IsActive = c.Boolean(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WorkOrderItemNeeds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WorkOrderDetailId = c.Int(),
                        WorkOrderId = c.Int(),
                        ItemId = c.Int(),
                        Quantity = c.Decimal(precision: 18, scale: 2),
                        RemainingNeedsQuantity = c.Decimal(precision: 18, scale: 2),
                        CalculatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkOrder", t => t.WorkOrderId)
                .ForeignKey("dbo.WorkOrderDetail", t => t.WorkOrderDetailId)
                .ForeignKey("dbo.Item", t => t.ItemId)
                .Index(t => t.WorkOrderDetailId)
                .Index(t => t.WorkOrderId)
                .Index(t => t.ItemId);
            
            CreateTable(
                "dbo.WorkOrderSerial",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WorkOrderDetailId = c.Int(),
                        WorkOrderId = c.Int(),
                        SerialNo = c.String(),
                        SerialType = c.Int(),
                        IsGeneratedBySignal = c.Boolean(),
                        SerialStatus = c.Int(),
                        FirstQuantity = c.Decimal(precision: 18, scale: 2),
                        LiveQuantity = c.Decimal(precision: 18, scale: 2),
                        InPackageQuantity = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                        ShiftId = c.Int(),
                        ItemReceiptDetailId = c.Int(),
                        QualityStatus = c.Int(),
                        QualityExplanation = c.String(),
                        TargetWarehouseId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Warehouse", t => t.TargetWarehouseId)
                .ForeignKey("dbo.WorkOrder", t => t.WorkOrderId)
                .ForeignKey("dbo.WorkOrderDetail", t => t.WorkOrderDetailId)
                .ForeignKey("dbo.Shift", t => t.ShiftId)
                .ForeignKey("dbo.ItemReceiptDetail", t => t.ItemReceiptDetailId)
                .Index(t => t.WorkOrderDetailId)
                .Index(t => t.WorkOrderId)
                .Index(t => t.ShiftId)
                .Index(t => t.ItemReceiptDetailId)
                .Index(t => t.TargetWarehouseId);
            
            CreateTable(
                "dbo.WorkOrderAllocation",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WorkOrderDetailId = c.Int(),
                        ItemReceiptDetailId = c.Int(),
                        Quantity = c.Decimal(precision: 18, scale: 2),
                        PackageQuantity = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkOrderDetail", t => t.WorkOrderDetailId)
                .ForeignKey("dbo.ItemReceiptDetail", t => t.ItemReceiptDetailId)
                .Index(t => t.WorkOrderDetailId)
                .Index(t => t.ItemReceiptDetailId);
            
            CreateTable(
                "dbo.ShiftTarget",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShiftId = c.Int(),
                        TargetDate = c.DateTime(),
                        TargetCount = c.Int(),
                        MachineId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Shift", t => t.ShiftId)
                .ForeignKey("dbo.Machine", t => t.MachineId)
                .Index(t => t.ShiftId)
                .Index(t => t.MachineId);
            
            CreateTable(
                "dbo.TransactionLog",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TransactionType = c.Int(),
                        TransactionDate = c.DateTime(),
                        Title = c.String(),
                        Message = c.String(),
                        UserId = c.Int(),
                        RecordId = c.Int(),
                        ObjectType = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserAuth",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AuthTypeId = c.Int(),
                        UserRoleId = c.Int(),
                        UserId = c.Int(),
                        IsGranted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserAuthType", t => t.AuthTypeId)
                .ForeignKey("dbo.UserRole", t => t.UserRoleId)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.AuthTypeId)
                .Index(t => t.UserRoleId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserAuthType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AuthTypeCode = c.String(),
                        AuthTypeName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserRole",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleName = c.String(),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.PlantId);
            
            CreateTable(
                "dbo.UserRoleSubscription",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserRoleId = c.Int(),
                        SubscriptionCategory = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserRole", t => t.UserRoleId)
                .Index(t => t.UserRoleId);
            
            CreateTable(
                "dbo.ItemRequestCategory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ItemUnit",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemId = c.Int(),
                        UnitId = c.Int(),
                        IsMainUnit = c.Boolean(),
                        MultiplierFactor = c.Decimal(precision: 18, scale: 2),
                        DividerFactor = c.Decimal(precision: 18, scale: 2),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UnitType", t => t.UnitId)
                .ForeignKey("dbo.Item", t => t.ItemId)
                .Index(t => t.ItemId)
                .Index(t => t.UnitId);
            
            CreateTable(
                "dbo.ProductRecipeDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductRecipeId = c.Int(),
                        ItemId = c.Int(),
                        ProcessType = c.Int(),
                        UnitId = c.Int(),
                        Quantity = c.Decimal(precision: 18, scale: 2),
                        WarehouseId = c.Int(),
                        WastagePercentage = c.Decimal(precision: 18, scale: 2),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductRecipe", t => t.ProductRecipeId)
                .ForeignKey("dbo.Warehouse", t => t.WarehouseId)
                .ForeignKey("dbo.UnitType", t => t.UnitId)
                .ForeignKey("dbo.Item", t => t.ItemId)
                .Index(t => t.ProductRecipeId)
                .Index(t => t.ItemId)
                .Index(t => t.UnitId)
                .Index(t => t.WarehouseId);
            
            CreateTable(
                "dbo.ProductRecipe",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductRecipeCode = c.String(),
                        Description = c.String(),
                        ProductRecipeType = c.Int(),
                        ProductId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                        IsActive = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Item", t => t.ProductId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.ItemWarehouse",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemId = c.Int(),
                        WarehouseId = c.Int(),
                        IsAllowed = c.Boolean(),
                        MinimumQuantity = c.Decimal(precision: 18, scale: 2),
                        MaximumQuantity = c.Decimal(precision: 18, scale: 2),
                        MinimumBehaviour = c.Int(),
                        MaximumBehaviour = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Warehouse", t => t.WarehouseId)
                .ForeignKey("dbo.Item", t => t.ItemId)
                .Index(t => t.ItemId)
                .Index(t => t.WarehouseId);
            
            CreateTable(
                "dbo.ProductQualityData",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MachineId = c.Int(),
                        ProductId = c.Int(),
                        ControlDate = c.DateTime(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                        Explanation = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Item", t => t.ProductId)
                .ForeignKey("dbo.Machine", t => t.MachineId)
                .Index(t => t.MachineId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.ProductQualityDataDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductQualityDataId = c.Int(),
                        ProductQualityPlanId = c.Int(),
                        OrderNo = c.Int(),
                        CheckResult = c.Boolean(),
                        NumericResult = c.Decimal(precision: 18, scale: 2),
                        IsOk = c.Boolean(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductQualityPlan", t => t.ProductQualityPlanId)
                .ForeignKey("dbo.ProductQualityData", t => t.ProductQualityDataId)
                .Index(t => t.ProductQualityDataId)
                .Index(t => t.ProductQualityPlanId);
            
            CreateTable(
                "dbo.ProductQualityPlan",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductQualityCode = c.String(),
                        CheckProperties = c.String(),
                        PeriodType = c.String(),
                        AcceptanceCriteria = c.String(),
                        ControlDevice = c.String(),
                        Method = c.String(),
                        Responsible = c.String(),
                        OrderNo = c.Int(),
                        CheckType = c.Int(nullable: false),
                        ToleranceMin = c.Decimal(precision: 18, scale: 2),
                        ToleranceMax = c.Decimal(precision: 18, scale: 2),
                        MoldTestFieldName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FirmAuthor",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirmId = c.Int(),
                        AuthorName = c.String(),
                        Title = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        Gsm = c.String(),
                        SendMailForPurchaseOrder = c.Boolean(),
                        SendMailForProduction = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Firm", t => t.FirmId)
                .Index(t => t.FirmId);
            
            CreateTable(
                "dbo.LayoutItem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MachineId = c.Int(),
                        PositionData = c.String(),
                        RotationData = c.String(),
                        ScalingData = c.String(),
                        Title = c.String(),
                        PlantId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .ForeignKey("dbo.Machine", t => t.MachineId)
                .Index(t => t.MachineId)
                .Index(t => t.PlantId);
            
            CreateTable(
                "dbo.MachineGroup",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MachineGroupCode = c.String(),
                        MachineGroupName = c.String(),
                        PlantId = c.Int(),
                        LayoutObjectTypeId = c.Int(),
                        IsProduction = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LayoutObjectType", t => t.LayoutObjectTypeId)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.PlantId)
                .Index(t => t.LayoutObjectTypeId);
            
            CreateTable(
                "dbo.LayoutObjectType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ObjectTypeCode = c.String(),
                        ObjectTypeName = c.String(),
                        ObjectData = c.Binary(),
                        DataTypeExtension = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RouteItem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RouteId = c.Int(),
                        ProcessId = c.Int(),
                        ProcessGroupId = c.Int(),
                        LineNumber = c.Int(),
                        Explanation = c.String(),
                        MachineId = c.Int(),
                        MachineGroupId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProcessGroup", t => t.ProcessGroupId)
                .ForeignKey("dbo.Process", t => t.ProcessId)
                .ForeignKey("dbo.Route", t => t.RouteId)
                .ForeignKey("dbo.MachineGroup", t => t.MachineGroupId)
                .ForeignKey("dbo.Machine", t => t.MachineId)
                .Index(t => t.RouteId)
                .Index(t => t.ProcessId)
                .Index(t => t.ProcessGroupId)
                .Index(t => t.MachineId)
                .Index(t => t.MachineGroupId);
            
            CreateTable(
                "dbo.Process",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProcessCode = c.String(),
                        ProcessName = c.String(),
                        PlantId = c.Int(),
                        IsActive = c.Boolean(),
                        TheoreticalDuration = c.Decimal(precision: 18, scale: 2),
                        ProcessGroupId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProcessGroup", t => t.ProcessGroupId)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.PlantId)
                .Index(t => t.ProcessGroupId);
            
            CreateTable(
                "dbo.ProcessGroup",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProcessGroupCode = c.String(),
                        ProcessGroupName = c.String(),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.PlantId);
            
            CreateTable(
                "dbo.Route",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RouteCode = c.String(),
                        RouteName = c.String(),
                        PlantId = c.Int(),
                        IsActive = c.Boolean(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.PlantId);
            
            CreateTable(
                "dbo.MachineMaintenanceInstruction",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MachineId = c.Int(),
                        UnitName = c.String(),
                        PeriodType = c.String(),
                        ToDoList = c.String(),
                        Responsible = c.String(),
                        LineNumber = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Machine", t => t.MachineId)
                .Index(t => t.MachineId);
            
            CreateTable(
                "dbo.MachineMaintenanceInstructionEntry",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InstructionId = c.Int(),
                        CreatedUserId = c.Int(),
                        CreatedDate = c.DateTime(),
                        IsChecked = c.Boolean(),
                        Explanation = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MachineMaintenanceInstruction", t => t.InstructionId)
                .Index(t => t.InstructionId);
            
            CreateTable(
                "dbo.AllocatedCode",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AllocatedCode = c.String(),
                        ObjectType = c.Int(),
                        CreatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Attachment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RecordId = c.Int(),
                        RecordType = c.Int(),
                        Description = c.String(),
                        BinaryContent = c.Binary(),
                        FileName = c.String(),
                        ContentType = c.String(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PrinterQueue",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PrinterId = c.Int(),
                        RecordType = c.Int(),
                        RecordId = c.Int(),
                        OrderNo = c.Int(),
                        AllocatedPrintData = c.String(),
                        CreatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SystemPrinter", t => t.PrinterId)
                .Index(t => t.PrinterId);
            
            CreateTable(
                "dbo.SystemPrinter",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PrinterCode = c.String(),
                        PrinterName = c.String(),
                        AccessPath = c.String(),
                        PlantId = c.Int(),
                        IsActive = c.Boolean(),
                        PageWidth = c.Decimal(precision: 18, scale: 2),
                        PageHeight = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ReportTemplate",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReportType = c.Int(),
                        ReportCode = c.String(),
                        ReportName = c.String(),
                        FileName = c.String(),
                        IsActive = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SectionSetting",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ColorCode = c.String(),
                        SectionGroupCode = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SystemParameter",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PrmCode = c.String(),
                        PrmValue = c.String(),
                        PlantId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UsageDocument",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentTitle = c.String(),
                        DocumentData = c.String(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PrinterQueue", "PrinterId", "dbo.SystemPrinter");
            DropForeignKey("dbo.WorkOrderDetail", "MachineId", "dbo.Machine");
            DropForeignKey("dbo.UserWorkOrderHistory", "MachineId", "dbo.Machine");
            DropForeignKey("dbo.ShiftTarget", "MachineId", "dbo.Machine");
            DropForeignKey("dbo.RouteItem", "MachineId", "dbo.Machine");
            DropForeignKey("dbo.ProductWastage", "MachineId", "dbo.Machine");
            DropForeignKey("dbo.ProductQualityData", "MachineId", "dbo.Machine");
            DropForeignKey("dbo.ProductionPosture", "MachineId", "dbo.Machine");
            DropForeignKey("dbo.MoldTest", "MachineId", "dbo.Machine");
            DropForeignKey("dbo.MachineSignal", "MachineId", "dbo.Machine");
            DropForeignKey("dbo.MachinePlan", "MachineId", "dbo.Machine");
            DropForeignKey("dbo.MachineMaintenanceInstruction", "MachineId", "dbo.Machine");
            DropForeignKey("dbo.MachineMaintenanceInstructionEntry", "InstructionId", "dbo.MachineMaintenanceInstruction");
            DropForeignKey("dbo.LayoutItem", "MachineId", "dbo.Machine");
            DropForeignKey("dbo.Incident", "MachineId", "dbo.Machine");
            DropForeignKey("dbo.Equipment", "MachineId", "dbo.Machine");
            DropForeignKey("dbo.WorkOrder", "PlantId", "dbo.Plant");
            DropForeignKey("dbo.Warehouse", "PlantId", "dbo.Plant");
            DropForeignKey("dbo.UserRole", "PlantId", "dbo.Plant");
            DropForeignKey("dbo.User", "PlantId", "dbo.Plant");
            DropForeignKey("dbo.UnitType", "PlantId", "dbo.Plant");
            DropForeignKey("dbo.SyncPoint", "PlantId", "dbo.Plant");
            DropForeignKey("dbo.Route", "PlantId", "dbo.Plant");
            DropForeignKey("dbo.ProcessGroup", "PlantId", "dbo.Plant");
            DropForeignKey("dbo.Process", "PlantId", "dbo.Plant");
            DropForeignKey("dbo.MachineGroup", "PlantId", "dbo.Plant");
            DropForeignKey("dbo.RouteItem", "MachineGroupId", "dbo.MachineGroup");
            DropForeignKey("dbo.RouteItem", "RouteId", "dbo.Route");
            DropForeignKey("dbo.RouteItem", "ProcessId", "dbo.Process");
            DropForeignKey("dbo.RouteItem", "ProcessGroupId", "dbo.ProcessGroup");
            DropForeignKey("dbo.Process", "ProcessGroupId", "dbo.ProcessGroup");
            DropForeignKey("dbo.ActualRouteHistory", "ProcessGroupId", "dbo.ProcessGroup");
            DropForeignKey("dbo.ActualRouteHistory", "ProcessId", "dbo.Process");
            DropForeignKey("dbo.Machine", "MachineGroupId", "dbo.MachineGroup");
            DropForeignKey("dbo.MachineGroup", "LayoutObjectTypeId", "dbo.LayoutObjectType");
            DropForeignKey("dbo.ActualRouteHistory", "MachineGroupId", "dbo.MachineGroup");
            DropForeignKey("dbo.LayoutItem", "PlantId", "dbo.Plant");
            DropForeignKey("dbo.ItemRequest", "PlantId", "dbo.Plant");
            DropForeignKey("dbo.ItemReceipt", "PlantId", "dbo.Plant");
            DropForeignKey("dbo.ItemOrder", "PlantId", "dbo.Plant");
            DropForeignKey("dbo.ItemGroup", "PlantId", "dbo.Plant");
            DropForeignKey("dbo.ItemCategory", "PlantId", "dbo.Plant");
            DropForeignKey("dbo.Item", "PlantId", "dbo.Plant");
            DropForeignKey("dbo.Invoice", "PlantId", "dbo.Plant");
            DropForeignKey("dbo.Firm", "PlantId", "dbo.Plant");
            DropForeignKey("dbo.WorkOrder", "FirmId", "dbo.Firm");
            DropForeignKey("dbo.Mold", "FirmId", "dbo.Firm");
            DropForeignKey("dbo.ItemReceipt", "FirmId", "dbo.Firm");
            DropForeignKey("dbo.ItemOrder", "FirmId", "dbo.Firm");
            DropForeignKey("dbo.Item", "SupplierFirmId", "dbo.Firm");
            DropForeignKey("dbo.Invoice", "FirmId", "dbo.Firm");
            DropForeignKey("dbo.FirmAuthor", "FirmId", "dbo.Firm");
            DropForeignKey("dbo.EntryQualityData", "FirmId", "dbo.Firm");
            DropForeignKey("dbo.EntryQualityData", "QualityPlanId", "dbo.EntryQualityPlan");
            DropForeignKey("dbo.EntryQualityDataDetail", "EntryQualityDataId", "dbo.EntryQualityData");
            DropForeignKey("dbo.ItemGroup", "ItemCategoryId", "dbo.ItemCategory");
            DropForeignKey("dbo.Item", "ItemCategoryId", "dbo.ItemCategory");
            DropForeignKey("dbo.WorkOrderItemNeeds", "ItemId", "dbo.Item");
            DropForeignKey("dbo.WorkOrderDetail", "ItemId", "dbo.Item");
            DropForeignKey("dbo.ProductWastage", "ProductId", "dbo.Item");
            DropForeignKey("dbo.ProductRecipeDetail", "ItemId", "dbo.Item");
            DropForeignKey("dbo.ProductRecipe", "ProductId", "dbo.Item");
            DropForeignKey("dbo.ProductQualityData", "ProductId", "dbo.Item");
            DropForeignKey("dbo.ProductQualityDataDetail", "ProductQualityDataId", "dbo.ProductQualityData");
            DropForeignKey("dbo.ProductQualityDataDetail", "ProductQualityPlanId", "dbo.ProductQualityPlan");
            DropForeignKey("dbo.MoldProduct", "ProductId", "dbo.Item");
            DropForeignKey("dbo.ItemWarehouse", "ItemId", "dbo.Item");
            DropForeignKey("dbo.ItemWarehouse", "WarehouseId", "dbo.Warehouse");
            DropForeignKey("dbo.ItemUnit", "ItemId", "dbo.Item");
            DropForeignKey("dbo.ItemSerial", "ItemId", "dbo.Item");
            DropForeignKey("dbo.ItemRequestDetail", "ItemId", "dbo.Item");
            DropForeignKey("dbo.ItemReceiptDetail", "ItemId", "dbo.Item");
            DropForeignKey("dbo.ItemOrderItemNeeds", "ItemId", "dbo.Item");
            DropForeignKey("dbo.ItemOrderDetail", "ItemId", "dbo.Item");
            DropForeignKey("dbo.WorkOrderDetail", "SaleOrderDetailId", "dbo.ItemOrderDetail");
            DropForeignKey("dbo.ItemReceiptDetail", "ItemOrderDetailId", "dbo.ItemOrderDetail");
            DropForeignKey("dbo.ItemOrderItemNeeds", "ItemOrderDetailId", "dbo.ItemOrderDetail");
            DropForeignKey("dbo.ItemOrderConsume", "ItemOrderDetailId", "dbo.ItemOrderDetail");
            DropForeignKey("dbo.ItemReceiptDetail", "ForexId", "dbo.ForexType");
            DropForeignKey("dbo.WorkOrderSerial", "ItemReceiptDetailId", "dbo.ItemReceiptDetail");
            DropForeignKey("dbo.WorkOrderAllocation", "ItemReceiptDetailId", "dbo.ItemReceiptDetail");
            DropForeignKey("dbo.ItemSerial", "ItemReceiptDetailId", "dbo.ItemReceiptDetail");
            DropForeignKey("dbo.ItemReceiptConsume", "ConsumerReceiptDetailId", "dbo.ItemReceiptDetail");
            DropForeignKey("dbo.ItemReceiptConsume", "ConsumedReceiptDetailId", "dbo.ItemReceiptDetail");
            DropForeignKey("dbo.ItemOrderConsume", "ConsumerReceiptDetailId", "dbo.ItemReceiptDetail");
            DropForeignKey("dbo.ItemOrderConsume", "ConsumedReceiptDetailId", "dbo.ItemReceiptDetail");
            DropForeignKey("dbo.ProductRecipeDetail", "UnitId", "dbo.UnitType");
            DropForeignKey("dbo.ProductRecipeDetail", "WarehouseId", "dbo.Warehouse");
            DropForeignKey("dbo.ProductRecipeDetail", "ProductRecipeId", "dbo.ProductRecipe");
            DropForeignKey("dbo.ItemUnit", "UnitId", "dbo.UnitType");
            DropForeignKey("dbo.ItemRequestDetail", "UnitId", "dbo.UnitType");
            DropForeignKey("dbo.ItemRequestDetail", "ItemRequestId", "dbo.ItemRequest");
            DropForeignKey("dbo.ItemRequest", "RequestCategoryId", "dbo.ItemRequestCategory");
            DropForeignKey("dbo.ItemRequestApproveLog", "ItemRequestId", "dbo.ItemRequest");
            DropForeignKey("dbo.UserAuth", "UserId", "dbo.User");
            DropForeignKey("dbo.UserRoleSubscription", "UserRoleId", "dbo.UserRole");
            DropForeignKey("dbo.UserAuth", "UserRoleId", "dbo.UserRole");
            DropForeignKey("dbo.User", "UserRoleId", "dbo.UserRole");
            DropForeignKey("dbo.UserAuth", "AuthTypeId", "dbo.UserAuthType");
            DropForeignKey("dbo.TransactionLog", "UserId", "dbo.User");
            DropForeignKey("dbo.Shift", "ShiftChiefId", "dbo.User");
            DropForeignKey("dbo.WorkOrderSerial", "ShiftId", "dbo.Shift");
            DropForeignKey("dbo.ShiftTarget", "ShiftId", "dbo.Shift");
            DropForeignKey("dbo.ProductWastage", "ShiftId", "dbo.Shift");
            DropForeignKey("dbo.ProductionPosture", "ShiftId", "dbo.Shift");
            DropForeignKey("dbo.MachineSignal", "ShiftId", "dbo.Shift");
            DropForeignKey("dbo.WorkOrderSerial", "WorkOrderDetailId", "dbo.WorkOrderDetail");
            DropForeignKey("dbo.WorkOrderItemNeeds", "WorkOrderDetailId", "dbo.WorkOrderDetail");
            DropForeignKey("dbo.WorkOrderControl", "WorkOrderDetailId", "dbo.WorkOrderDetail");
            DropForeignKey("dbo.WorkOrderAllocation", "WorkOrderDetailId", "dbo.WorkOrderDetail");
            DropForeignKey("dbo.WorkOrderSerial", "WorkOrderId", "dbo.WorkOrder");
            DropForeignKey("dbo.WorkOrderSerial", "TargetWarehouseId", "dbo.Warehouse");
            DropForeignKey("dbo.WorkOrderItemNeeds", "WorkOrderId", "dbo.WorkOrder");
            DropForeignKey("dbo.WorkOrderDetail", "WorkOrderId", "dbo.WorkOrder");
            DropForeignKey("dbo.WorkOrderControl", "WorkOrderId", "dbo.WorkOrder");
            DropForeignKey("dbo.WorkOrderControl", "ControlTypeId", "dbo.WorkOrderControlType");
            DropForeignKey("dbo.WorkOrder", "WorkOrderCategoryId", "dbo.WorkOrderCategory");
            DropForeignKey("dbo.UserWorkOrderHistory", "WorkOrderDetailId", "dbo.WorkOrderDetail");
            DropForeignKey("dbo.ProductWastage", "WorkOrderDetailId", "dbo.WorkOrderDetail");
            DropForeignKey("dbo.ProductionPosture", "WorkOrderDetailId", "dbo.WorkOrderDetail");
            DropForeignKey("dbo.ProductionPosture", "PostureCategoryId", "dbo.PostureCategory");
            DropForeignKey("dbo.MachineSignal", "WorkOrderDetailId", "dbo.WorkOrderDetail");
            DropForeignKey("dbo.MachinePlan", "WorkOrderDetailId", "dbo.WorkOrderDetail");
            DropForeignKey("dbo.ItemSerial", "WorkOrderDetailId", "dbo.WorkOrderDetail");
            DropForeignKey("dbo.WorkOrderDetail", "DyeId", "dbo.Dye");
            DropForeignKey("dbo.MoldTest", "DyeId", "dbo.Dye");
            DropForeignKey("dbo.WorkOrderDetail", "MoldTestId", "dbo.MoldTest");
            DropForeignKey("dbo.WorkOrderDetail", "MoldId", "dbo.Mold");
            DropForeignKey("dbo.MoldTest", "MoldId", "dbo.Mold");
            DropForeignKey("dbo.MoldProduct", "MoldId", "dbo.Mold");
            DropForeignKey("dbo.Mold", "MoldItemId", "dbo.Item");
            DropForeignKey("dbo.Item", "MoldId", "dbo.Mold");
            DropForeignKey("dbo.DeliveryPlan", "WorkOrderDetailId", "dbo.WorkOrderDetail");
            DropForeignKey("dbo.ActualRouteHistory", "WorkOrderDetailId", "dbo.WorkOrderDetail");
            DropForeignKey("dbo.Incident", "ShiftId", "dbo.Shift");
            DropForeignKey("dbo.Incident", "IncidentCategoryId", "dbo.IncidentCategory");
            DropForeignKey("dbo.Notification", "UserId", "dbo.User");
            DropForeignKey("dbo.ItemRequestApproveLog", "ActorUserId", "dbo.User");
            DropForeignKey("dbo.ItemRequest", "CreatedUserId", "dbo.User");
            DropForeignKey("dbo.Equipment", "ResponsibleUserId", "dbo.User");
            DropForeignKey("dbo.ItemOrder", "OutWarehouseId", "dbo.Warehouse");
            DropForeignKey("dbo.ItemOrder", "InWarehouseId", "dbo.Warehouse");
            DropForeignKey("dbo.ItemOrder", "ItemRequestId", "dbo.ItemRequest");
            DropForeignKey("dbo.ItemReceipt", "ItemOrderId", "dbo.ItemOrder");
            DropForeignKey("dbo.ItemReceipt", "OutWarehouseId", "dbo.Warehouse");
            DropForeignKey("dbo.ItemReceipt", "InWarehouseId", "dbo.Warehouse");
            DropForeignKey("dbo.ItemReceipt", "SyncPointId", "dbo.SyncPoint");
            DropForeignKey("dbo.ItemOrder", "SyncPointId", "dbo.SyncPoint");
            DropForeignKey("dbo.ItemReceiptDetail", "ItemReceiptId", "dbo.ItemReceipt");
            DropForeignKey("dbo.ItemReceipt", "InvoiceId", "dbo.Invoice");
            DropForeignKey("dbo.ItemOrderItemNeeds", "ItemOrderId", "dbo.ItemOrder");
            DropForeignKey("dbo.ItemOrderDetail", "ItemOrderId", "dbo.ItemOrder");
            DropForeignKey("dbo.ItemOrderDetail", "ItemRequestDetailId", "dbo.ItemRequestDetail");
            DropForeignKey("dbo.ItemReceiptDetail", "UnitId", "dbo.UnitType");
            DropForeignKey("dbo.ItemReceiptConsume", "UnitId", "dbo.UnitType");
            DropForeignKey("dbo.ItemOrderDetail", "UnitId", "dbo.UnitType");
            DropForeignKey("dbo.ItemOrderConsume", "UnitId", "dbo.UnitType");
            DropForeignKey("dbo.EntryQualityData", "ItemEntryDetailId", "dbo.ItemReceiptDetail");
            DropForeignKey("dbo.ItemOrderDetail", "ForexId", "dbo.ForexType");
            DropForeignKey("dbo.ForexHistory", "ForexId", "dbo.ForexType");
            DropForeignKey("dbo.ItemLiveStatus", "ItemId", "dbo.Item");
            DropForeignKey("dbo.ItemLiveStatus", "WarehouseId", "dbo.Warehouse");
            DropForeignKey("dbo.Item", "ItemGroupId", "dbo.ItemGroup");
            DropForeignKey("dbo.EntryQualityPlan", "ItemGroupId", "dbo.ItemGroup");
            DropForeignKey("dbo.EntryQualityData", "ItemId", "dbo.Item");
            DropForeignKey("dbo.EntryQualityPlan", "ItemCategoryId", "dbo.ItemCategory");
            DropForeignKey("dbo.EntryQualityPlanDetail", "EntryQualityPlanId", "dbo.EntryQualityPlan");
            DropForeignKey("dbo.EntryQualityDataDetail", "EntryQualityPlanDetailId", "dbo.EntryQualityPlanDetail");
            DropForeignKey("dbo.EntryQualityData", "QualityPlanDetailId", "dbo.EntryQualityPlanDetail");
            DropForeignKey("dbo.EquipmentCategory", "PlantId", "dbo.Plant");
            DropForeignKey("dbo.Equipment", "PlantId", "dbo.Plant");
            DropForeignKey("dbo.Equipment", "EquipmentCategoryId", "dbo.EquipmentCategory");
            DropForeignKey("dbo.ActualRouteHistory", "MachineId", "dbo.Machine");
            DropIndex("dbo.PrinterQueue", new[] { "PrinterId" });
            DropIndex("dbo.MachineMaintenanceInstructionEntry", new[] { "InstructionId" });
            DropIndex("dbo.MachineMaintenanceInstruction", new[] { "MachineId" });
            DropIndex("dbo.Route", new[] { "PlantId" });
            DropIndex("dbo.ProcessGroup", new[] { "PlantId" });
            DropIndex("dbo.Process", new[] { "ProcessGroupId" });
            DropIndex("dbo.Process", new[] { "PlantId" });
            DropIndex("dbo.RouteItem", new[] { "MachineGroupId" });
            DropIndex("dbo.RouteItem", new[] { "MachineId" });
            DropIndex("dbo.RouteItem", new[] { "ProcessGroupId" });
            DropIndex("dbo.RouteItem", new[] { "ProcessId" });
            DropIndex("dbo.RouteItem", new[] { "RouteId" });
            DropIndex("dbo.MachineGroup", new[] { "LayoutObjectTypeId" });
            DropIndex("dbo.MachineGroup", new[] { "PlantId" });
            DropIndex("dbo.LayoutItem", new[] { "PlantId" });
            DropIndex("dbo.LayoutItem", new[] { "MachineId" });
            DropIndex("dbo.FirmAuthor", new[] { "FirmId" });
            DropIndex("dbo.ProductQualityDataDetail", new[] { "ProductQualityPlanId" });
            DropIndex("dbo.ProductQualityDataDetail", new[] { "ProductQualityDataId" });
            DropIndex("dbo.ProductQualityData", new[] { "ProductId" });
            DropIndex("dbo.ProductQualityData", new[] { "MachineId" });
            DropIndex("dbo.ItemWarehouse", new[] { "WarehouseId" });
            DropIndex("dbo.ItemWarehouse", new[] { "ItemId" });
            DropIndex("dbo.ProductRecipe", new[] { "ProductId" });
            DropIndex("dbo.ProductRecipeDetail", new[] { "WarehouseId" });
            DropIndex("dbo.ProductRecipeDetail", new[] { "UnitId" });
            DropIndex("dbo.ProductRecipeDetail", new[] { "ItemId" });
            DropIndex("dbo.ProductRecipeDetail", new[] { "ProductRecipeId" });
            DropIndex("dbo.ItemUnit", new[] { "UnitId" });
            DropIndex("dbo.ItemUnit", new[] { "ItemId" });
            DropIndex("dbo.UserRoleSubscription", new[] { "UserRoleId" });
            DropIndex("dbo.UserRole", new[] { "PlantId" });
            DropIndex("dbo.UserAuth", new[] { "UserId" });
            DropIndex("dbo.UserAuth", new[] { "UserRoleId" });
            DropIndex("dbo.UserAuth", new[] { "AuthTypeId" });
            DropIndex("dbo.TransactionLog", new[] { "UserId" });
            DropIndex("dbo.ShiftTarget", new[] { "MachineId" });
            DropIndex("dbo.ShiftTarget", new[] { "ShiftId" });
            DropIndex("dbo.WorkOrderAllocation", new[] { "ItemReceiptDetailId" });
            DropIndex("dbo.WorkOrderAllocation", new[] { "WorkOrderDetailId" });
            DropIndex("dbo.WorkOrderSerial", new[] { "TargetWarehouseId" });
            DropIndex("dbo.WorkOrderSerial", new[] { "ItemReceiptDetailId" });
            DropIndex("dbo.WorkOrderSerial", new[] { "ShiftId" });
            DropIndex("dbo.WorkOrderSerial", new[] { "WorkOrderId" });
            DropIndex("dbo.WorkOrderSerial", new[] { "WorkOrderDetailId" });
            DropIndex("dbo.WorkOrderItemNeeds", new[] { "ItemId" });
            DropIndex("dbo.WorkOrderItemNeeds", new[] { "WorkOrderId" });
            DropIndex("dbo.WorkOrderItemNeeds", new[] { "WorkOrderDetailId" });
            DropIndex("dbo.WorkOrderControl", new[] { "ControlTypeId" });
            DropIndex("dbo.WorkOrderControl", new[] { "WorkOrderDetailId" });
            DropIndex("dbo.WorkOrderControl", new[] { "WorkOrderId" });
            DropIndex("dbo.WorkOrder", new[] { "WorkOrderCategoryId" });
            DropIndex("dbo.WorkOrder", new[] { "PlantId" });
            DropIndex("dbo.WorkOrder", new[] { "FirmId" });
            DropIndex("dbo.UserWorkOrderHistory", new[] { "MachineId" });
            DropIndex("dbo.UserWorkOrderHistory", new[] { "WorkOrderDetailId" });
            DropIndex("dbo.ProductWastage", new[] { "ShiftId" });
            DropIndex("dbo.ProductWastage", new[] { "MachineId" });
            DropIndex("dbo.ProductWastage", new[] { "ProductId" });
            DropIndex("dbo.ProductWastage", new[] { "WorkOrderDetailId" });
            DropIndex("dbo.ProductionPosture", new[] { "ShiftId" });
            DropIndex("dbo.ProductionPosture", new[] { "PostureCategoryId" });
            DropIndex("dbo.ProductionPosture", new[] { "WorkOrderDetailId" });
            DropIndex("dbo.ProductionPosture", new[] { "MachineId" });
            DropIndex("dbo.MachinePlan", new[] { "WorkOrderDetailId" });
            DropIndex("dbo.MachinePlan", new[] { "MachineId" });
            DropIndex("dbo.ItemSerial", new[] { "WorkOrderDetailId" });
            DropIndex("dbo.ItemSerial", new[] { "ItemReceiptDetailId" });
            DropIndex("dbo.ItemSerial", new[] { "ItemId" });
            DropIndex("dbo.MoldProduct", new[] { "ProductId" });
            DropIndex("dbo.MoldProduct", new[] { "MoldId" });
            DropIndex("dbo.Mold", new[] { "MoldItemId" });
            DropIndex("dbo.Mold", new[] { "FirmId" });
            DropIndex("dbo.MoldTest", new[] { "MoldId" });
            DropIndex("dbo.MoldTest", new[] { "DyeId" });
            DropIndex("dbo.MoldTest", new[] { "MachineId" });
            DropIndex("dbo.DeliveryPlan", new[] { "WorkOrderDetailId" });
            DropIndex("dbo.WorkOrderDetail", new[] { "SaleOrderDetailId" });
            DropIndex("dbo.WorkOrderDetail", new[] { "MoldTestId" });
            DropIndex("dbo.WorkOrderDetail", new[] { "MachineId" });
            DropIndex("dbo.WorkOrderDetail", new[] { "DyeId" });
            DropIndex("dbo.WorkOrderDetail", new[] { "MoldId" });
            DropIndex("dbo.WorkOrderDetail", new[] { "ItemId" });
            DropIndex("dbo.WorkOrderDetail", new[] { "WorkOrderId" });
            DropIndex("dbo.MachineSignal", new[] { "ShiftId" });
            DropIndex("dbo.MachineSignal", new[] { "WorkOrderDetailId" });
            DropIndex("dbo.MachineSignal", new[] { "MachineId" });
            DropIndex("dbo.Incident", new[] { "ShiftId" });
            DropIndex("dbo.Incident", new[] { "IncidentCategoryId" });
            DropIndex("dbo.Incident", new[] { "MachineId" });
            DropIndex("dbo.Shift", new[] { "ShiftChiefId" });
            DropIndex("dbo.Notification", new[] { "UserId" });
            DropIndex("dbo.User", new[] { "UserRoleId" });
            DropIndex("dbo.User", new[] { "PlantId" });
            DropIndex("dbo.ItemRequestApproveLog", new[] { "ActorUserId" });
            DropIndex("dbo.ItemRequestApproveLog", new[] { "ItemRequestId" });
            DropIndex("dbo.SyncPoint", new[] { "PlantId" });
            DropIndex("dbo.Invoice", new[] { "PlantId" });
            DropIndex("dbo.Invoice", new[] { "FirmId" });
            DropIndex("dbo.ItemReceipt", new[] { "InvoiceId" });
            DropIndex("dbo.ItemReceipt", new[] { "ItemOrderId" });
            DropIndex("dbo.ItemReceipt", new[] { "SyncPointId" });
            DropIndex("dbo.ItemReceipt", new[] { "PlantId" });
            DropIndex("dbo.ItemReceipt", new[] { "OutWarehouseId" });
            DropIndex("dbo.ItemReceipt", new[] { "InWarehouseId" });
            DropIndex("dbo.ItemReceipt", new[] { "FirmId" });
            DropIndex("dbo.ItemOrderItemNeeds", new[] { "ItemId" });
            DropIndex("dbo.ItemOrderItemNeeds", new[] { "ItemOrderId" });
            DropIndex("dbo.ItemOrderItemNeeds", new[] { "ItemOrderDetailId" });
            DropIndex("dbo.ItemOrder", new[] { "SyncPointId" });
            DropIndex("dbo.ItemOrder", new[] { "ItemRequestId" });
            DropIndex("dbo.ItemOrder", new[] { "PlantId" });
            DropIndex("dbo.ItemOrder", new[] { "OutWarehouseId" });
            DropIndex("dbo.ItemOrder", new[] { "InWarehouseId" });
            DropIndex("dbo.ItemOrder", new[] { "FirmId" });
            DropIndex("dbo.ItemRequest", new[] { "CreatedUserId" });
            DropIndex("dbo.ItemRequest", new[] { "PlantId" });
            DropIndex("dbo.ItemRequest", new[] { "RequestCategoryId" });
            DropIndex("dbo.ItemRequestDetail", new[] { "UnitId" });
            DropIndex("dbo.ItemRequestDetail", new[] { "ItemId" });
            DropIndex("dbo.ItemRequestDetail", new[] { "ItemRequestId" });
            DropIndex("dbo.ItemReceiptConsume", new[] { "UnitId" });
            DropIndex("dbo.ItemReceiptConsume", new[] { "ConsumerReceiptDetailId" });
            DropIndex("dbo.ItemReceiptConsume", new[] { "ConsumedReceiptDetailId" });
            DropIndex("dbo.UnitType", new[] { "PlantId" });
            DropIndex("dbo.ItemOrderConsume", new[] { "UnitId" });
            DropIndex("dbo.ItemOrderConsume", new[] { "ConsumedReceiptDetailId" });
            DropIndex("dbo.ItemOrderConsume", new[] { "ConsumerReceiptDetailId" });
            DropIndex("dbo.ItemOrderConsume", new[] { "ItemOrderDetailId" });
            DropIndex("dbo.ItemReceiptDetail", new[] { "ItemOrderDetailId" });
            DropIndex("dbo.ItemReceiptDetail", new[] { "ForexId" });
            DropIndex("dbo.ItemReceiptDetail", new[] { "UnitId" });
            DropIndex("dbo.ItemReceiptDetail", new[] { "ItemId" });
            DropIndex("dbo.ItemReceiptDetail", new[] { "ItemReceiptId" });
            DropIndex("dbo.ForexHistory", new[] { "ForexId" });
            DropIndex("dbo.ItemOrderDetail", new[] { "ItemRequestDetailId" });
            DropIndex("dbo.ItemOrderDetail", new[] { "ForexId" });
            DropIndex("dbo.ItemOrderDetail", new[] { "UnitId" });
            DropIndex("dbo.ItemOrderDetail", new[] { "ItemId" });
            DropIndex("dbo.ItemOrderDetail", new[] { "ItemOrderId" });
            DropIndex("dbo.Warehouse", new[] { "PlantId" });
            DropIndex("dbo.ItemLiveStatus", new[] { "WarehouseId" });
            DropIndex("dbo.ItemLiveStatus", new[] { "ItemId" });
            DropIndex("dbo.ItemGroup", new[] { "PlantId" });
            DropIndex("dbo.ItemGroup", new[] { "ItemCategoryId" });
            DropIndex("dbo.Item", new[] { "MoldId" });
            DropIndex("dbo.Item", new[] { "PlantId" });
            DropIndex("dbo.Item", new[] { "SupplierFirmId" });
            DropIndex("dbo.Item", new[] { "ItemGroupId" });
            DropIndex("dbo.Item", new[] { "ItemCategoryId" });
            DropIndex("dbo.ItemCategory", new[] { "PlantId" });
            DropIndex("dbo.EntryQualityPlan", new[] { "ItemCategoryId" });
            DropIndex("dbo.EntryQualityPlan", new[] { "ItemGroupId" });
            DropIndex("dbo.EntryQualityPlanDetail", new[] { "EntryQualityPlanId" });
            DropIndex("dbo.EntryQualityDataDetail", new[] { "EntryQualityPlanDetailId" });
            DropIndex("dbo.EntryQualityDataDetail", new[] { "EntryQualityDataId" });
            DropIndex("dbo.EntryQualityData", new[] { "ItemId" });
            DropIndex("dbo.EntryQualityData", new[] { "FirmId" });
            DropIndex("dbo.EntryQualityData", new[] { "ItemEntryDetailId" });
            DropIndex("dbo.EntryQualityData", new[] { "QualityPlanDetailId" });
            DropIndex("dbo.EntryQualityData", new[] { "QualityPlanId" });
            DropIndex("dbo.Firm", new[] { "PlantId" });
            DropIndex("dbo.EquipmentCategory", new[] { "PlantId" });
            DropIndex("dbo.Equipment", new[] { "EquipmentCategoryId" });
            DropIndex("dbo.Equipment", new[] { "ResponsibleUserId" });
            DropIndex("dbo.Equipment", new[] { "MachineId" });
            DropIndex("dbo.Equipment", new[] { "PlantId" });
            DropIndex("dbo.Machine", new[] { "MachineGroupId" });
            DropIndex("dbo.ActualRouteHistory", new[] { "MachineGroupId" });
            DropIndex("dbo.ActualRouteHistory", new[] { "MachineId" });
            DropIndex("dbo.ActualRouteHistory", new[] { "ProcessGroupId" });
            DropIndex("dbo.ActualRouteHistory", new[] { "ProcessId" });
            DropIndex("dbo.ActualRouteHistory", new[] { "WorkOrderDetailId" });
            DropTable("dbo.UsageDocument");
            DropTable("dbo.SystemParameter");
            DropTable("dbo.SectionSetting");
            DropTable("dbo.ReportTemplate");
            DropTable("dbo.SystemPrinter");
            DropTable("dbo.PrinterQueue");
            DropTable("dbo.Attachment");
            DropTable("dbo.AllocatedCode");
            DropTable("dbo.MachineMaintenanceInstructionEntry");
            DropTable("dbo.MachineMaintenanceInstruction");
            DropTable("dbo.Route");
            DropTable("dbo.ProcessGroup");
            DropTable("dbo.Process");
            DropTable("dbo.RouteItem");
            DropTable("dbo.LayoutObjectType");
            DropTable("dbo.MachineGroup");
            DropTable("dbo.LayoutItem");
            DropTable("dbo.FirmAuthor");
            DropTable("dbo.ProductQualityPlan");
            DropTable("dbo.ProductQualityDataDetail");
            DropTable("dbo.ProductQualityData");
            DropTable("dbo.ItemWarehouse");
            DropTable("dbo.ProductRecipe");
            DropTable("dbo.ProductRecipeDetail");
            DropTable("dbo.ItemUnit");
            DropTable("dbo.ItemRequestCategory");
            DropTable("dbo.UserRoleSubscription");
            DropTable("dbo.UserRole");
            DropTable("dbo.UserAuthType");
            DropTable("dbo.UserAuth");
            DropTable("dbo.TransactionLog");
            DropTable("dbo.ShiftTarget");
            DropTable("dbo.WorkOrderAllocation");
            DropTable("dbo.WorkOrderSerial");
            DropTable("dbo.WorkOrderItemNeeds");
            DropTable("dbo.WorkOrderControlType");
            DropTable("dbo.WorkOrderControl");
            DropTable("dbo.WorkOrderCategory");
            DropTable("dbo.WorkOrder");
            DropTable("dbo.UserWorkOrderHistory");
            DropTable("dbo.ProductWastage");
            DropTable("dbo.PostureCategory");
            DropTable("dbo.ProductionPosture");
            DropTable("dbo.MachinePlan");
            DropTable("dbo.ItemSerial");
            DropTable("dbo.MoldProduct");
            DropTable("dbo.Mold");
            DropTable("dbo.MoldTest");
            DropTable("dbo.Dye");
            DropTable("dbo.DeliveryPlan");
            DropTable("dbo.WorkOrderDetail");
            DropTable("dbo.MachineSignal");
            DropTable("dbo.IncidentCategory");
            DropTable("dbo.Incident");
            DropTable("dbo.Shift");
            DropTable("dbo.Notification");
            DropTable("dbo.User");
            DropTable("dbo.ItemRequestApproveLog");
            DropTable("dbo.SyncPoint");
            DropTable("dbo.Invoice");
            DropTable("dbo.ItemReceipt");
            DropTable("dbo.ItemOrderItemNeeds");
            DropTable("dbo.ItemOrder");
            DropTable("dbo.ItemRequest");
            DropTable("dbo.ItemRequestDetail");
            DropTable("dbo.ItemReceiptConsume");
            DropTable("dbo.UnitType");
            DropTable("dbo.ItemOrderConsume");
            DropTable("dbo.ItemReceiptDetail");
            DropTable("dbo.ForexHistory");
            DropTable("dbo.ForexType");
            DropTable("dbo.ItemOrderDetail");
            DropTable("dbo.Warehouse");
            DropTable("dbo.ItemLiveStatus");
            DropTable("dbo.ItemGroup");
            DropTable("dbo.Item");
            DropTable("dbo.ItemCategory");
            DropTable("dbo.EntryQualityPlan");
            DropTable("dbo.EntryQualityPlanDetail");
            DropTable("dbo.EntryQualityDataDetail");
            DropTable("dbo.EntryQualityData");
            DropTable("dbo.Firm");
            DropTable("dbo.Plant");
            DropTable("dbo.EquipmentCategory");
            DropTable("dbo.Equipment");
            DropTable("dbo.Machine");
            DropTable("dbo.ActualRouteHistory");
        }
    }
}
