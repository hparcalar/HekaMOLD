namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OfferRoutePriceColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemOfferDetail", "RoutePrice", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemOfferDetail", "RoutePrice");
        }
    }
}
