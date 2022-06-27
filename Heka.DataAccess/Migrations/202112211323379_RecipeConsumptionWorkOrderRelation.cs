namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RecipeConsumptionWorkOrderRelation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemReceipt", "WorkOrderDetailId", c => c.Int());
            CreateIndex("dbo.ItemReceipt", "WorkOrderDetailId");
            AddForeignKey("dbo.ItemReceipt", "WorkOrderDetailId", "dbo.WorkOrderDetail", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemReceipt", "WorkOrderDetailId", "dbo.WorkOrderDetail");
            DropIndex("dbo.ItemReceipt", new[] { "WorkOrderDetailId" });
            DropColumn("dbo.ItemReceipt", "WorkOrderDetailId");
        }
    }
}
