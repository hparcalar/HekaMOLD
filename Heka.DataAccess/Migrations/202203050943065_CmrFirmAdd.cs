namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CmrFirmAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "CmrShipperCityId", c => c.Int());
            AddColumn("dbo.ItemLoad", "CmrBuyerCityId", c => c.Int());
            AddColumn("dbo.ItemLoad", "CmrShipperCountryId", c => c.Int());
            AddColumn("dbo.ItemLoad", "CmrBuyerCountryId", c => c.Int());
            AddColumn("dbo.ItemLoad", "CmrShipperFirmId", c => c.Int());
            AddColumn("dbo.ItemLoad", "CmrBuyerFirmId", c => c.Int());
            CreateIndex("dbo.ItemLoad", "CmrShipperCityId");
            CreateIndex("dbo.ItemLoad", "CmrBuyerCityId");
            CreateIndex("dbo.ItemLoad", "CmrShipperCountryId");
            CreateIndex("dbo.ItemLoad", "CmrBuyerCountryId");
            CreateIndex("dbo.ItemLoad", "CmrShipperFirmId");
            CreateIndex("dbo.ItemLoad", "CmrBuyerFirmId");
            AddForeignKey("dbo.ItemLoad", "CmrBuyerCountryId", "dbo.Country", "Id");
            AddForeignKey("dbo.ItemLoad", "CmrShipperCountryId", "dbo.Country", "Id");
            AddForeignKey("dbo.ItemLoad", "CmrBuyerCityId", "dbo.City", "Id");
            AddForeignKey("dbo.ItemLoad", "CmrShipperCityId", "dbo.City", "Id");
            AddForeignKey("dbo.ItemLoad", "CmrBuyerFirmId", "dbo.Firm", "Id");
            AddForeignKey("dbo.ItemLoad", "CmrShipperFirmId", "dbo.Firm", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemLoad", "CmrShipperFirmId", "dbo.Firm");
            DropForeignKey("dbo.ItemLoad", "CmrBuyerFirmId", "dbo.Firm");
            DropForeignKey("dbo.ItemLoad", "CmrShipperCityId", "dbo.City");
            DropForeignKey("dbo.ItemLoad", "CmrBuyerCityId", "dbo.City");
            DropForeignKey("dbo.ItemLoad", "CmrShipperCountryId", "dbo.Country");
            DropForeignKey("dbo.ItemLoad", "CmrBuyerCountryId", "dbo.Country");
            DropIndex("dbo.ItemLoad", new[] { "CmrBuyerFirmId" });
            DropIndex("dbo.ItemLoad", new[] { "CmrShipperFirmId" });
            DropIndex("dbo.ItemLoad", new[] { "CmrBuyerCountryId" });
            DropIndex("dbo.ItemLoad", new[] { "CmrShipperCountryId" });
            DropIndex("dbo.ItemLoad", new[] { "CmrBuyerCityId" });
            DropIndex("dbo.ItemLoad", new[] { "CmrShipperCityId" });
            DropColumn("dbo.ItemLoad", "CmrBuyerFirmId");
            DropColumn("dbo.ItemLoad", "CmrShipperFirmId");
            DropColumn("dbo.ItemLoad", "CmrBuyerCountryId");
            DropColumn("dbo.ItemLoad", "CmrShipperCountryId");
            DropColumn("dbo.ItemLoad", "CmrBuyerCityId");
            DropColumn("dbo.ItemLoad", "CmrShipperCityId");
        }
    }
}
