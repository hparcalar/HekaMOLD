namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateShipperFirmExplanationAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "ShipperFirmExplanation", c => c.String());
            AddColumn("dbo.ItemLoad", "BuyerFirmExplanation", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemLoad", "BuyerFirmExplanation");
            DropColumn("dbo.ItemLoad", "ShipperFirmExplanation");
        }
    }
}
