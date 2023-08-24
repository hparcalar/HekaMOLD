namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConsumptionRelationToProductionReceipt : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemReceipt", "ConsumptionReceiptId", c => c.Int());
            CreateIndex("dbo.ItemReceipt", "ConsumptionReceiptId");
            AddForeignKey("dbo.ItemReceipt", "ConsumptionReceiptId", "dbo.ItemReceipt", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemReceipt", "ConsumptionReceiptId", "dbo.ItemReceipt");
            DropIndex("dbo.ItemReceipt", new[] { "ConsumptionReceiptId" });
            DropColumn("dbo.ItemReceipt", "ConsumptionReceiptId");
        }
    }
}
