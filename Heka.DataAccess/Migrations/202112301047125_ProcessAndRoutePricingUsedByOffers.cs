namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProcessAndRoutePricingUsedByOffers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemOfferDetailRoutePricing",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemOfferDetailId = c.Int(),
                        RouteItemId = c.Int(),
                        UnitPrice = c.Decimal(precision: 18, scale: 2),
                        ForexId = c.Int(),
                        TotalPrice = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ForexType", t => t.ForexId)
                .ForeignKey("dbo.RouteItem", t => t.RouteItemId)
                .ForeignKey("dbo.ItemOfferDetail", t => t.ItemOfferDetailId)
                .Index(t => t.ItemOfferDetailId)
                .Index(t => t.RouteItemId)
                .Index(t => t.ForexId);
            
            AddColumn("dbo.Route", "UnitPrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Route", "ForexId", c => c.Int());
            AddColumn("dbo.Process", "UnitPrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Process", "ForexId", c => c.Int());
            CreateIndex("dbo.Process", "ForexId");
            CreateIndex("dbo.Route", "ForexId");
            AddForeignKey("dbo.Process", "ForexId", "dbo.ForexType", "Id");
            AddForeignKey("dbo.Route", "ForexId", "dbo.ForexType", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemOfferDetailRoutePricing", "ItemOfferDetailId", "dbo.ItemOfferDetail");
            DropForeignKey("dbo.ItemOfferDetailRoutePricing", "RouteItemId", "dbo.RouteItem");
            DropForeignKey("dbo.Route", "ForexId", "dbo.ForexType");
            DropForeignKey("dbo.Process", "ForexId", "dbo.ForexType");
            DropForeignKey("dbo.ItemOfferDetailRoutePricing", "ForexId", "dbo.ForexType");
            DropIndex("dbo.Route", new[] { "ForexId" });
            DropIndex("dbo.Process", new[] { "ForexId" });
            DropIndex("dbo.ItemOfferDetailRoutePricing", new[] { "ForexId" });
            DropIndex("dbo.ItemOfferDetailRoutePricing", new[] { "RouteItemId" });
            DropIndex("dbo.ItemOfferDetailRoutePricing", new[] { "ItemOfferDetailId" });
            DropColumn("dbo.Process", "ForexId");
            DropColumn("dbo.Process", "UnitPrice");
            DropColumn("dbo.Route", "ForexId");
            DropColumn("dbo.Route", "UnitPrice");
            DropTable("dbo.ItemOfferDetailRoutePricing");
        }
    }
}
