namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SaleOfferRoutePrice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemOfferDetail", "RouteId", c => c.Int());
            CreateIndex("dbo.ItemOfferDetail", "RouteId");
            AddForeignKey("dbo.ItemOfferDetail", "RouteId", "dbo.Route", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemOfferDetail", "RouteId", "dbo.Route");
            DropIndex("dbo.ItemOfferDetail", new[] { "RouteId" });
            DropColumn("dbo.ItemOfferDetail", "RouteId");
        }
    }
}
