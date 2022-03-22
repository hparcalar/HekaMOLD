namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateShipperAndBuyerFirmAdressAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "BuyerFirmAddress", c => c.String());
            AddColumn("dbo.ItemLoad", "ShipperFirmAddress", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemLoad", "ShipperFirmAddress");
            DropColumn("dbo.ItemLoad", "BuyerFirmAddress");
        }
    }
}
