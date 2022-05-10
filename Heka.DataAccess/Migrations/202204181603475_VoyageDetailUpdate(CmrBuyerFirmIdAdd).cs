namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageDetailUpdateCmrBuyerFirmIdAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VoyageDetail", "CmrShipperFirmId", c => c.Int());
            AddColumn("dbo.VoyageDetail", "CmrBuyerFirmId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VoyageDetail", "CmrBuyerFirmId");
            DropColumn("dbo.VoyageDetail", "CmrShipperFirmId");
        }
    }
}
