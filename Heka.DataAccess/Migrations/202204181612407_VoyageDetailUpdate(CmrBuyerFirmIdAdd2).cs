namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageDetailUpdateCmrBuyerFirmIdAdd2 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.VoyageDetail", "CmrShipperFirmId");
            CreateIndex("dbo.VoyageDetail", "CmrBuyerFirmId");
            AddForeignKey("dbo.VoyageDetail", "CmrBuyerFirmId", "dbo.Firm", "Id");
            AddForeignKey("dbo.VoyageDetail", "CmrShipperFirmId", "dbo.Firm", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VoyageDetail", "CmrShipperFirmId", "dbo.Firm");
            DropForeignKey("dbo.VoyageDetail", "CmrBuyerFirmId", "dbo.Firm");
            DropIndex("dbo.VoyageDetail", new[] { "CmrBuyerFirmId" });
            DropIndex("dbo.VoyageDetail", new[] { "CmrShipperFirmId" });
        }
    }
}
