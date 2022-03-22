namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateFirmReelOwnerAndFirmManufacturer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "ManufacturerFirmId", c => c.Int());
            AddColumn("dbo.ItemLoad", "ReelOwnerFirmId", c => c.Int());
            CreateIndex("dbo.ItemLoad", "ManufacturerFirmId");
            CreateIndex("dbo.ItemLoad", "ReelOwnerFirmId");
            AddForeignKey("dbo.ItemLoad", "ManufacturerFirmId", "dbo.Firm", "Id");
            AddForeignKey("dbo.ItemLoad", "ReelOwnerFirmId", "dbo.Firm", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemLoad", "ReelOwnerFirmId", "dbo.Firm");
            DropForeignKey("dbo.ItemLoad", "ManufacturerFirmId", "dbo.Firm");
            DropIndex("dbo.ItemLoad", new[] { "ReelOwnerFirmId" });
            DropIndex("dbo.ItemLoad", new[] { "ManufacturerFirmId" });
            DropColumn("dbo.ItemLoad", "ReelOwnerFirmId");
            DropColumn("dbo.ItemLoad", "ManufacturerFirmId");
        }
    }
}
