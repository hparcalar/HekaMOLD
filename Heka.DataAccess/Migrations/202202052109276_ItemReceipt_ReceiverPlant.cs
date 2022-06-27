namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemReceipt_ReceiverPlant : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemReceipt", "ReceiverPlantId", c => c.Int());
            CreateIndex("dbo.ItemReceipt", "ReceiverPlantId");
            AddForeignKey("dbo.ItemReceipt", "ReceiverPlantId", "dbo.Plant", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemReceipt", "ReceiverPlantId", "dbo.Plant");
            DropIndex("dbo.ItemReceipt", new[] { "ReceiverPlantId" });
            DropColumn("dbo.ItemReceipt", "ReceiverPlantId");
        }
    }
}
