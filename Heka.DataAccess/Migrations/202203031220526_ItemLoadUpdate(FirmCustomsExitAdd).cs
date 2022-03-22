namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateFirmCustomsExitAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "FirmCustomsExitId", c => c.Int());
            CreateIndex("dbo.ItemLoad", "FirmCustomsExitId");
            AddForeignKey("dbo.ItemLoad", "FirmCustomsExitId", "dbo.Firm", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemLoad", "FirmCustomsExitId", "dbo.Firm");
            DropIndex("dbo.ItemLoad", new[] { "FirmCustomsExitId" });
            DropColumn("dbo.ItemLoad", "FirmCustomsExitId");
        }
    }
}
