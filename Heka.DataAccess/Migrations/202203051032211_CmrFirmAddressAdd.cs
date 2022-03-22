namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CmrFirmAddressAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "CmrBuyerFirmAddress", c => c.String());
            AddColumn("dbo.ItemLoad", "CmrShipperFirmAddress", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemLoad", "CmrShipperFirmAddress");
            DropColumn("dbo.ItemLoad", "CmrBuyerFirmAddress");
        }
    }
}
