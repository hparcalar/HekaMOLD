namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageDetailUpdateReelOwnerFirmAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VoyageDetail", "ReelOwnerFirmId", c => c.Int());
            CreateIndex("dbo.VoyageDetail", "ReelOwnerFirmId");
            AddForeignKey("dbo.VoyageDetail", "ReelOwnerFirmId", "dbo.Firm", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VoyageDetail", "ReelOwnerFirmId", "dbo.Firm");
            DropIndex("dbo.VoyageDetail", new[] { "ReelOwnerFirmId" });
            DropColumn("dbo.VoyageDetail", "ReelOwnerFirmId");
        }
    }
}
