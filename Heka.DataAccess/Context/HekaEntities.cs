﻿namespace Heka.DataAccess.Context
{
    using Heka.DataAccess.Migrations;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public partial class HekaEntities : DbContext
    {
        public HekaEntities()
            : base("name=HekaEntities")
        {
            //Database.SetInitializer<HekaEntities>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public virtual DbSet<AllocatedCode> AllocatedCode { get; set; }
        public virtual DbSet<Attachment> Attachment { get; set; }
        public virtual DbSet<DeliveryPlan> DeliveryPlan { get; set; }
        public virtual DbSet<Dye> Dye { get; set; }
        public virtual DbSet<EntryQualityData> EntryQualityData { get; set; }
        public virtual DbSet<EntryQualityDataDetail> EntryQualityDataDetail { get; set; }
        public virtual DbSet<EntryQualityPlan> EntryQualityPlan { get; set; }
        public virtual DbSet<EntryQualityPlanDetail> EntryQualityPlanDetail { get; set; }
        public virtual DbSet<Equipment> Equipment { get; set; }
        public virtual DbSet<EquipmentCategory> EquipmentCategory { get; set; }
        public virtual DbSet<Firm> Firm { get; set; }
        public virtual DbSet<FirmAuthor> FirmAuthor { get; set; }
        public virtual DbSet<ForexHistory> ForexHistory { get; set; }
        public virtual DbSet<ForexType> ForexType { get; set; }
        public virtual DbSet<Incident> Incident { get; set; }
        public virtual DbSet<IncidentCategory> IncidentCategory { get; set; }
        public virtual DbSet<Invoice> Invoice { get; set; }
        public virtual DbSet<Item> Item { get; set; }
        public virtual DbSet<ItemPrice> ItemPrice { get; set; }
        public virtual DbSet<ItemCategory> ItemCategory { get; set; }
        public virtual DbSet<ItemGroup> ItemGroup { get; set; }
        public virtual DbSet<ItemLiveStatus> ItemLiveStatus { get; set; }
        public virtual DbSet<ItemOrder> ItemOrder { get; set; }
        public virtual DbSet<ItemOrderDetail> ItemOrderDetail { get; set; }
        public virtual DbSet<ItemOrderItemNeeds> ItemOrderItemNeeds { get; set; }
        public virtual DbSet<ItemReceipt> ItemReceipt { get; set; }
        public virtual DbSet<ItemReceiptConsume> ItemReceiptConsume { get; set; }
        public virtual DbSet<ItemReceiptDetail> ItemReceiptDetail { get; set; }
        public virtual DbSet<ItemRequest> ItemRequest { get; set; }
        public virtual DbSet<ItemRequestApproveLog> ItemRequestApproveLog { get; set; }
        public virtual DbSet<ItemRequestCategory> ItemRequestCategory { get; set; }
        public virtual DbSet<ItemRequestDetail> ItemRequestDetail { get; set; }
        public virtual DbSet<ItemUnit> ItemUnit { get; set; }
        public virtual DbSet<ItemWarehouse> ItemWarehouse { get; set; }
        public virtual DbSet<LayoutItem> LayoutItem { get; set; }
        public virtual DbSet<LayoutObjectType> LayoutObjectType { get; set; }
        public virtual DbSet<MachineMaintenanceInstruction> MachineMaintenanceInstruction { get; set; }
        public virtual DbSet<MachineMaintenanceInstructionEntry> MachineMaintenanceInstructionEntry { get; set; }
        public virtual DbSet<MachinePlan> MachinePlan { get; set; }
        public virtual DbSet<MachineSignal> MachineSignal { get; set; }
        public virtual DbSet<Mold> Mold { get; set; }
        public virtual DbSet<MoldProduct> MoldProduct { get; set; }
        public virtual DbSet<MoldTest> MoldTest { get; set; }
        public virtual DbSet<Plant> Plant { get; set; }
        public virtual DbSet<PrinterQueue> PrinterQueue { get; set; }
        public virtual DbSet<ProductionPosture> ProductionPosture { get; set; }
        public virtual DbSet<ProductQualityData> ProductQualityData { get; set; }
        public virtual DbSet<ProductQualityDataDetail> ProductQualityDataDetail { get; set; }
        public virtual DbSet<ProductQualityPlan> ProductQualityPlan { get; set; }
        public virtual DbSet<ProductRecipe> ProductRecipe { get; set; }
        public virtual DbSet<ProductRecipeDetail> ProductRecipeDetail { get; set; }
        public virtual DbSet<ReportTemplate> ReportTemplate { get; set; }
        public virtual DbSet<SectionSetting> SectionSetting { get; set; }
        public virtual DbSet<SyncPoint> SyncPoint { get; set; }
        public virtual DbSet<SystemParameter> SystemParameter { get; set; }
        public virtual DbSet<SystemPrinter> SystemPrinter { get; set; }
        public virtual DbSet<TransactionLog> TransactionLog { get; set; }
        public virtual DbSet<UnitType> UnitType { get; set; }
        public virtual DbSet<UsageDocument> UsageDocument { get; set; }
        public virtual DbSet<UserAuth> UserAuth { get; set; }
        public virtual DbSet<UserAuthType> UserAuthType { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<UserWorkOrderHistory> UserWorkOrderHistory { get; set; }
        public virtual DbSet<Warehouse> Warehouse { get; set; }
        public virtual DbSet<WorkOrderAllocation> WorkOrderAllocation { get; set; }
        public virtual DbSet<WorkOrderControl> WorkOrderControl { get; set; }
        public virtual DbSet<WorkOrderControlType> WorkOrderControlType { get; set; }
        public virtual DbSet<WorkOrderItemNeeds> WorkOrderItemNeeds { get; set; }
        public virtual DbSet<UserRoleSubscription> UserRoleSubscription { get; set; }
        public virtual DbSet<Notification> Notification { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Shift> Shift { get; set; }
        public virtual DbSet<ShiftTarget> ShiftTarget { get; set; }
        public virtual DbSet<ActualRouteHistory> ActualRouteHistory { get; set; }
        public virtual DbSet<Process> Process { get; set; }
        public virtual DbSet<ProcessGroup> ProcessGroup { get; set; }
        public virtual DbSet<Route> Route { get; set; }
        public virtual DbSet<RouteItem> RouteItem { get; set; }
        public virtual DbSet<Machine> Machine { get; set; }
        public virtual DbSet<PostureCategory> PostureCategory { get; set; }
        public virtual DbSet<WorkOrderSerial> WorkOrderSerial { get; set; }
        public virtual DbSet<MachineGroup> MachineGroup { get; set; }
        public virtual DbSet<WorkOrder> WorkOrder { get; set; }
        public virtual DbSet<WorkOrderCategory> WorkOrderCategory { get; set; }
        public virtual DbSet<ItemSerial> ItemSerial { get; set; }
        public virtual DbSet<ItemOrderConsume> ItemOrderConsume { get; set; }
        public virtual DbSet<WorkOrderDetail> WorkOrderDetail { get; set; }
        public virtual DbSet<ProductWastage> ProductWastage { get; set; }
        public virtual DbSet<ContractWorkFlow> ContractWorkFlow { get; set; }
        public virtual DbSet<PreProcessType> PreProcessType { get; set; }
        public virtual DbSet<MachinePreProcess> MachinePreProcess { get; set; }
        public virtual DbSet<MachinePreProcessHistory> MachinePreProcessHistory { get; set; }
    }
}