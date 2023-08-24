namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CountingSerialItemRelation : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.CountingReceiptSerial", "ItemId");
            AddForeignKey("dbo.CountingReceiptSerial", "ItemId", "dbo.Item", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CountingReceiptSerial", "ItemId", "dbo.Item");
            DropIndex("dbo.CountingReceiptSerial", new[] { "ItemId" });
        }
    }
}
