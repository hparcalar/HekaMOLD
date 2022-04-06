namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SheetMultipleUsagesBySaleOrders : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemOrderSheetUsage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemOrderDetailId = c.Int(),
                        ItemOrderSheetId = c.Int(),
                        Quantity = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ItemOrderSheet", t => t.ItemOrderSheetId)
                .ForeignKey("dbo.ItemOrderDetail", t => t.ItemOrderDetailId)
                .Index(t => t.ItemOrderDetailId)
                .Index(t => t.ItemOrderSheetId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemOrderSheetUsage", "ItemOrderDetailId", "dbo.ItemOrderDetail");
            DropForeignKey("dbo.ItemOrderSheetUsage", "ItemOrderSheetId", "dbo.ItemOrderSheet");
            DropIndex("dbo.ItemOrderSheetUsage", new[] { "ItemOrderSheetId" });
            DropIndex("dbo.ItemOrderSheetUsage", new[] { "ItemOrderDetailId" });
            DropTable("dbo.ItemOrderSheetUsage");
        }
    }
}
