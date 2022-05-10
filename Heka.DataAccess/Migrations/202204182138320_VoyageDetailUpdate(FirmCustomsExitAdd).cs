namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageDetailUpdateFirmCustomsExitAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VoyageDetail", "FirmCustomsExitId", c => c.Int());
            CreateIndex("dbo.VoyageDetail", "FirmCustomsExitId");
            AddForeignKey("dbo.VoyageDetail", "FirmCustomsExitId", "dbo.Firm", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VoyageDetail", "FirmCustomsExitId", "dbo.Firm");
            DropIndex("dbo.VoyageDetail", new[] { "FirmCustomsExitId" });
            DropColumn("dbo.VoyageDetail", "FirmCustomsExitId");
        }
    }
}
