namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageDetailUpdateManufacturerFirmIdAdd2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VoyageDetail", "ManufacturerFirmId", c => c.Int());
            CreateIndex("dbo.VoyageDetail", "ManufacturerFirmId");
            AddForeignKey("dbo.VoyageDetail", "ManufacturerFirmId", "dbo.Firm", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VoyageDetail", "ManufacturerFirmId", "dbo.Firm");
            DropIndex("dbo.VoyageDetail", new[] { "ManufacturerFirmId" });
            DropColumn("dbo.VoyageDetail", "ManufacturerFirmId");
        }
    }
}
