namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemDemands : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemDemand",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemId = c.Int(),
                        DemandQuantity = c.Decimal(precision: 18, scale: 2),
                        SuppliedQuantity = c.Decimal(precision: 18, scale: 2),
                        WorkOrderDetailId = c.Int(),
                        Explanation = c.String(),
                        DemandDate = c.DateTime(),
                        SupplyDate = c.DateTime(),
                        DemandedUserId = c.Int(),
                        SupplierUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.DemandedUserId)
                .ForeignKey("dbo.Item", t => t.ItemId)
                .ForeignKey("dbo.User", t => t.SupplierUserId)
                .ForeignKey("dbo.WorkOrderDetail", t => t.WorkOrderDetailId)
                .Index(t => t.ItemId)
                .Index(t => t.WorkOrderDetailId)
                .Index(t => t.DemandedUserId)
                .Index(t => t.SupplierUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemDemand", "WorkOrderDetailId", "dbo.WorkOrderDetail");
            DropForeignKey("dbo.ItemDemand", "SupplierUserId", "dbo.User");
            DropForeignKey("dbo.ItemDemand", "ItemId", "dbo.Item");
            DropForeignKey("dbo.ItemDemand", "DemandedUserId", "dbo.User");
            DropIndex("dbo.ItemDemand", new[] { "SupplierUserId" });
            DropIndex("dbo.ItemDemand", new[] { "DemandedUserId" });
            DropIndex("dbo.ItemDemand", new[] { "WorkOrderDetailId" });
            DropIndex("dbo.ItemDemand", new[] { "ItemId" });
            DropTable("dbo.ItemDemand");
        }
    }
}
