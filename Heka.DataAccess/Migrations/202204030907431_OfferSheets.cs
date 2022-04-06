namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OfferSheets : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemOfferSheet",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemOfferDetailId = c.Int(),
                        ItemOfferId = c.Int(),
                        SheetName = c.String(),
                        SheetNo = c.Int(),
                        SheetVisual = c.Binary(),
                        PerSheetTime = c.DateTime(),
                        Quantity = c.Int(),
                        Thickness = c.Int(),
                        Eff = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ItemOffer", t => t.ItemOfferId)
                .ForeignKey("dbo.ItemOfferDetail", t => t.ItemOfferDetailId)
                .Index(t => t.ItemOfferDetailId)
                .Index(t => t.ItemOfferId);
            
            CreateTable(
                "dbo.ItemOrderSheet",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemOrderDetailId = c.Int(),
                        ItemOrderId = c.Int(),
                        SheetName = c.String(),
                        SheetNo = c.Int(),
                        SheetVisual = c.Binary(),
                        PerSheetTime = c.DateTime(),
                        Quantity = c.Int(),
                        Thickness = c.Int(),
                        Eff = c.Decimal(precision: 18, scale: 2),
                        SheetStatus = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ItemOrder", t => t.ItemOrderId)
                .ForeignKey("dbo.ItemOrderDetail", t => t.ItemOrderDetailId)
                .Index(t => t.ItemOrderDetailId)
                .Index(t => t.ItemOrderId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemOrderSheet", "ItemOrderDetailId", "dbo.ItemOrderDetail");
            DropForeignKey("dbo.ItemOrderSheet", "ItemOrderId", "dbo.ItemOrder");
            DropForeignKey("dbo.ItemOfferSheet", "ItemOfferDetailId", "dbo.ItemOfferDetail");
            DropForeignKey("dbo.ItemOfferSheet", "ItemOfferId", "dbo.ItemOffer");
            DropIndex("dbo.ItemOrderSheet", new[] { "ItemOrderId" });
            DropIndex("dbo.ItemOrderSheet", new[] { "ItemOrderDetailId" });
            DropIndex("dbo.ItemOfferSheet", new[] { "ItemOfferId" });
            DropIndex("dbo.ItemOfferSheet", new[] { "ItemOfferDetailId" });
            DropTable("dbo.ItemOrderSheet");
            DropTable("dbo.ItemOfferSheet");
        }
    }
}
