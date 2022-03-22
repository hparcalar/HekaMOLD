namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemOrderUpdateReelOwnerFirmAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemOrder", "ReelOwnerFirmId", c => c.Int());
            AddColumn("dbo.ItemOrder", "ArrivalWarehouseDate", c => c.DateTime());
            AddColumn("dbo.ItemOrder", "ReceiptFromCustomerDate", c => c.DateTime());
            AddColumn("dbo.ItemOrder", "EstimatedUplodDate", c => c.DateTime());
            AddColumn("dbo.ItemOrder", "IntendedArrivalDate", c => c.DateTime());
            CreateIndex("dbo.ItemOrder", "ReelOwnerFirmId");
            AddForeignKey("dbo.ItemOrder", "ReelOwnerFirmId", "dbo.Firm", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemOrder", "ReelOwnerFirmId", "dbo.Firm");
            DropIndex("dbo.ItemOrder", new[] { "ReelOwnerFirmId" });
            DropColumn("dbo.ItemOrder", "IntendedArrivalDate");
            DropColumn("dbo.ItemOrder", "EstimatedUplodDate");
            DropColumn("dbo.ItemOrder", "ReceiptFromCustomerDate");
            DropColumn("dbo.ItemOrder", "ArrivalWarehouseDate");
            DropColumn("dbo.ItemOrder", "ReelOwnerFirmId");
        }
    }
}
