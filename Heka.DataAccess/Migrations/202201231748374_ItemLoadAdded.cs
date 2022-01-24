namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemLoadDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemLoadId = c.Int(),
                        LineNumber = c.Int(),
                        ItemId = c.Int(),
                        UnitId = c.Int(),
                        Quantity = c.Decimal(precision: 18, scale: 2),
                        NetQuantity = c.Decimal(precision: 18, scale: 2),
                        GrossQuantity = c.Decimal(precision: 18, scale: 2),
                        UnitPrice = c.Decimal(precision: 18, scale: 2),
                        ShortWidth = c.Int(),
                        LongWidth = c.Int(),
                        Volume = c.Decimal(precision: 18, scale: 2),
                        Height = c.Int(),
                        Weight = c.Int(),
                        PackageInNumber = c.Int(),
                        Stackable = c.Boolean(),
                        Ladametre = c.Decimal(precision: 18, scale: 2),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ItemLoad", t => t.ItemLoadId)
                .ForeignKey("dbo.UnitType", t => t.UnitId)
                .ForeignKey("dbo.Item", t => t.ItemId)
                .Index(t => t.ItemLoadId)
                .Index(t => t.ItemId)
                .Index(t => t.UnitId);
            
            CreateTable(
                "dbo.ItemLoad",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LoadCode = c.String(),
                        LoadDate = c.DateTime(),
                        DischargeDate = c.DateTime(),
                        OrderLoadStatusType = c.Int(),
                        OveralWeight = c.Decimal(precision: 18, scale: 2),
                        OveralVolume = c.Decimal(precision: 18, scale: 2),
                        OveralLadametre = c.Decimal(precision: 18, scale: 2),
                        OverallTotal = c.Decimal(precision: 18, scale: 2),
                        CalculationTypePrice = c.Decimal(precision: 18, scale: 2),
                        Explanation = c.String(),
                        ShipperFirmId = c.Int(),
                        BuyerFirmId = c.Int(),
                        ItemOrderId = c.Int(),
                        PlantId = c.Int(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ItemOrder", t => t.ItemOrderId)
                .ForeignKey("dbo.User", t => t.CreatedUserId)
                .ForeignKey("dbo.Firm", t => t.BuyerFirmId)
                .ForeignKey("dbo.Firm", t => t.ShipperFirmId)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.ShipperFirmId)
                .Index(t => t.BuyerFirmId)
                .Index(t => t.ItemOrderId)
                .Index(t => t.PlantId)
                .Index(t => t.CreatedUserId);
            
            AddColumn("dbo.ItemOrderDetail", "ItemLoadDetailId", c => c.Int());
            CreateIndex("dbo.ItemOrderDetail", "ItemLoadDetailId");
            AddForeignKey("dbo.ItemOrderDetail", "ItemLoadDetailId", "dbo.ItemLoadDetail", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemLoad", "PlantId", "dbo.Plant");
            DropForeignKey("dbo.ItemLoad", "ShipperFirmId", "dbo.Firm");
            DropForeignKey("dbo.ItemLoad", "BuyerFirmId", "dbo.Firm");
            DropForeignKey("dbo.ItemLoadDetail", "ItemId", "dbo.Item");
            DropForeignKey("dbo.ItemOrderDetail", "ItemLoadDetailId", "dbo.ItemLoadDetail");
            DropForeignKey("dbo.ItemLoad", "CreatedUserId", "dbo.User");
            DropForeignKey("dbo.ItemLoadDetail", "UnitId", "dbo.UnitType");
            DropForeignKey("dbo.ItemLoad", "ItemOrderId", "dbo.ItemOrder");
            DropForeignKey("dbo.ItemLoadDetail", "ItemLoadId", "dbo.ItemLoad");
            DropIndex("dbo.ItemOrderDetail", new[] { "ItemLoadDetailId" });
            DropIndex("dbo.ItemLoad", new[] { "CreatedUserId" });
            DropIndex("dbo.ItemLoad", new[] { "PlantId" });
            DropIndex("dbo.ItemLoad", new[] { "ItemOrderId" });
            DropIndex("dbo.ItemLoad", new[] { "BuyerFirmId" });
            DropIndex("dbo.ItemLoad", new[] { "ShipperFirmId" });
            DropIndex("dbo.ItemLoadDetail", new[] { "UnitId" });
            DropIndex("dbo.ItemLoadDetail", new[] { "ItemId" });
            DropIndex("dbo.ItemLoadDetail", new[] { "ItemLoadId" });
            DropColumn("dbo.ItemOrderDetail", "ItemLoadDetailId");
            DropTable("dbo.ItemLoad");
            DropTable("dbo.ItemLoadDetail");
        }
    }
}
