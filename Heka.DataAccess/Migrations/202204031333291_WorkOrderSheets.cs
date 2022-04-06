namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WorkOrderSheets : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WorkOrderDetail", "ItemOrderSheetId", c => c.Int());
            AddColumn("dbo.WorkOrderSerial", "ItemId", c => c.Int());
            CreateIndex("dbo.WorkOrderDetail", "ItemOrderSheetId");
            CreateIndex("dbo.WorkOrderSerial", "ItemId");
            AddForeignKey("dbo.WorkOrderSerial", "ItemId", "dbo.Item", "Id");
            AddForeignKey("dbo.WorkOrderDetail", "ItemOrderSheetId", "dbo.ItemOrderSheet", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WorkOrderDetail", "ItemOrderSheetId", "dbo.ItemOrderSheet");
            DropForeignKey("dbo.WorkOrderSerial", "ItemId", "dbo.Item");
            DropIndex("dbo.WorkOrderSerial", new[] { "ItemId" });
            DropIndex("dbo.WorkOrderDetail", new[] { "ItemOrderSheetId" });
            DropColumn("dbo.WorkOrderSerial", "ItemId");
            DropColumn("dbo.WorkOrderDetail", "ItemOrderSheetId");
        }
    }
}
