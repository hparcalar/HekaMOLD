namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateFirmCustomsArrivalAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "FirmCustomsArrivalId", c => c.Int());
            CreateIndex("dbo.ItemLoad", "FirmCustomsArrivalId");
            AddForeignKey("dbo.ItemLoad", "FirmCustomsArrivalId", "dbo.Firm", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemLoad", "FirmCustomsArrivalId", "dbo.Firm");
            DropIndex("dbo.ItemLoad", new[] { "FirmCustomsArrivalId" });
            DropColumn("dbo.ItemLoad", "FirmCustomsArrivalId");
        }
    }
}
