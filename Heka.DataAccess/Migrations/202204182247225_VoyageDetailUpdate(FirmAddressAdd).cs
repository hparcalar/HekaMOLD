namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageDetailUpdateFirmAddressAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VoyageDetail", "BuyerFirmAddress", c => c.String());
            AddColumn("dbo.VoyageDetail", "ShipperFirmAddress", c => c.String());
            AddColumn("dbo.VoyageDetail", "CmrBuyerFirmAddress", c => c.String());
            AddColumn("dbo.VoyageDetail", "CmrShipperFirmAddress", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VoyageDetail", "CmrShipperFirmAddress");
            DropColumn("dbo.VoyageDetail", "CmrBuyerFirmAddress");
            DropColumn("dbo.VoyageDetail", "ShipperFirmAddress");
            DropColumn("dbo.VoyageDetail", "BuyerFirmAddress");
        }
    }
}
