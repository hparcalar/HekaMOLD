namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CountingReceipt : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CountingReceipt",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CountingDate = c.DateTime(),
                        ReceiptNo = c.String(),
                        PlantId = c.Int(),
                        CountingStatus = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.PlantId);
            
            CreateTable(
                "dbo.CountingReceiptDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CountingReceiptId = c.Int(),
                        ItemId = c.Int(),
                        Quantity = c.Decimal(precision: 18, scale: 2),
                        PackageQuantity = c.Int(),
                        WarehouseId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Item", t => t.ItemId)
                .ForeignKey("dbo.Warehouse", t => t.WarehouseId)
                .ForeignKey("dbo.CountingReceipt", t => t.CountingReceiptId)
                .Index(t => t.CountingReceiptId)
                .Index(t => t.ItemId)
                .Index(t => t.WarehouseId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CountingReceipt", "PlantId", "dbo.Plant");
            DropForeignKey("dbo.CountingReceiptDetail", "CountingReceiptId", "dbo.CountingReceipt");
            DropForeignKey("dbo.CountingReceiptDetail", "WarehouseId", "dbo.Warehouse");
            DropForeignKey("dbo.CountingReceiptDetail", "ItemId", "dbo.Item");
            DropIndex("dbo.CountingReceiptDetail", new[] { "WarehouseId" });
            DropIndex("dbo.CountingReceiptDetail", new[] { "ItemId" });
            DropIndex("dbo.CountingReceiptDetail", new[] { "CountingReceiptId" });
            DropIndex("dbo.CountingReceipt", new[] { "PlantId" });
            DropTable("dbo.CountingReceiptDetail");
            DropTable("dbo.CountingReceipt");
        }
    }
}
