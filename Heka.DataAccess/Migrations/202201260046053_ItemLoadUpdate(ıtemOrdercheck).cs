namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateıtemOrdercheck : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "DocumentNo", c => c.String());
            AddColumn("dbo.ItemLoad", "OrderUploadType", c => c.Int());
            AddColumn("dbo.ItemLoad", "OrderUploadPointType", c => c.Int());
            AddColumn("dbo.ItemLoad", "OrderTransactionDirectionType", c => c.Int());
            AddColumn("dbo.ItemLoad", "OrderCalculationType", c => c.Int());
            AddColumn("dbo.ItemLoad", "LoadOutDate", c => c.DateTime());
            AddColumn("dbo.ItemLoad", "CustomerFirmId", c => c.Int());
            AddColumn("dbo.ItemLoad", "EntryCustomsId", c => c.Int());
            AddColumn("dbo.ItemLoad", "ExitCustomsId", c => c.Int());
            CreateIndex("dbo.ItemLoad", "CustomerFirmId");
            CreateIndex("dbo.ItemLoad", "EntryCustomsId");
            CreateIndex("dbo.ItemLoad", "ExitCustomsId");
            AddForeignKey("dbo.ItemLoad", "EntryCustomsId", "dbo.Customs", "Id");
            AddForeignKey("dbo.ItemLoad", "ExitCustomsId", "dbo.Customs", "Id");
            AddForeignKey("dbo.ItemLoad", "CustomerFirmId", "dbo.Firm", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemLoad", "CustomerFirmId", "dbo.Firm");
            DropForeignKey("dbo.ItemLoad", "ExitCustomsId", "dbo.Customs");
            DropForeignKey("dbo.ItemLoad", "EntryCustomsId", "dbo.Customs");
            DropIndex("dbo.ItemLoad", new[] { "ExitCustomsId" });
            DropIndex("dbo.ItemLoad", new[] { "EntryCustomsId" });
            DropIndex("dbo.ItemLoad", new[] { "CustomerFirmId" });
            DropColumn("dbo.ItemLoad", "ExitCustomsId");
            DropColumn("dbo.ItemLoad", "EntryCustomsId");
            DropColumn("dbo.ItemLoad", "CustomerFirmId");
            DropColumn("dbo.ItemLoad", "LoadOutDate");
            DropColumn("dbo.ItemLoad", "OrderCalculationType");
            DropColumn("dbo.ItemLoad", "OrderTransactionDirectionType");
            DropColumn("dbo.ItemLoad", "OrderUploadPointType");
            DropColumn("dbo.ItemLoad", "OrderUploadType");
            DropColumn("dbo.ItemLoad", "DocumentNo");
        }
    }
}
