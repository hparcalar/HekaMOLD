namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemOffers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemOfferDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemOfferId = c.Int(),
                        ItemId = c.Int(),
                        ItemExplanation = c.String(),
                        QualityExplanation = c.String(),
                        ItemVisual = c.Binary(),
                        Quantity = c.Decimal(precision: 18, scale: 2),
                        UnitPrice = c.Decimal(precision: 18, scale: 2),
                        TotalPrice = c.Decimal(precision: 18, scale: 2),
                        SheetWeight = c.Decimal(precision: 18, scale: 2),
                        LaborCost = c.Decimal(precision: 18, scale: 2),
                        WastageWeight = c.Decimal(precision: 18, scale: 2),
                        ProfitRate = c.Decimal(precision: 18, scale: 2),
                        CreditMonths = c.Int(),
                        CreditRate = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Item", t => t.ItemId)
                .ForeignKey("dbo.ItemOffer", t => t.ItemOfferId)
                .Index(t => t.ItemOfferId)
                .Index(t => t.ItemId);
            
            CreateTable(
                "dbo.ItemOffer",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OfferNo = c.String(),
                        OfferType = c.Int(nullable: false),
                        FirmId = c.Int(),
                        OfferDate = c.DateTime(),
                        Explanation = c.String(),
                        TotalQuantity = c.Decimal(precision: 18, scale: 2),
                        TotalPrice = c.Decimal(precision: 18, scale: 2),
                        PlantId = c.Int(),
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
            
            AddColumn("dbo.ItemOrderDetail", "ItemOfferDetailId", c => c.Int());
            CreateIndex("dbo.ItemOrderDetail", "ItemOfferDetailId");
            AddForeignKey("dbo.ItemOrderDetail", "ItemOfferDetailId", "dbo.ItemOfferDetail", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemOrderDetail", "ItemOfferDetailId", "dbo.ItemOfferDetail");
            DropForeignKey("dbo.ItemOffer", "PlantId", "dbo.Plant");
            DropForeignKey("dbo.ItemOfferDetail", "ItemOfferId", "dbo.ItemOffer");
            DropForeignKey("dbo.ItemOffer", "FirmId", "dbo.Firm");
            DropForeignKey("dbo.ItemOfferDetail", "ItemId", "dbo.Item");
            DropIndex("dbo.ItemOffer", new[] { "PlantId" });
            DropIndex("dbo.ItemOffer", new[] { "FirmId" });
            DropIndex("dbo.ItemOfferDetail", new[] { "ItemId" });
            DropIndex("dbo.ItemOfferDetail", new[] { "ItemOfferId" });
            DropIndex("dbo.ItemOrderDetail", new[] { "ItemOfferDetailId" });
            DropColumn("dbo.ItemOrderDetail", "ItemOfferDetailId");
            DropTable("dbo.ItemOffer");
            DropTable("dbo.ItemOfferDetail");
        }
    }
}
