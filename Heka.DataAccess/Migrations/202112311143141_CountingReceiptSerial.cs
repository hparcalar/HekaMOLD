namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CountingReceiptSerial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CountingReceiptSerial",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Barcode = c.String(),
                        ItemId = c.Int(),
                        Quantity = c.Decimal(precision: 18, scale: 2),
                        CountingReceiptDetailId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CountingReceiptDetail", t => t.CountingReceiptDetailId)
                .Index(t => t.CountingReceiptDetailId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CountingReceiptSerial", "CountingReceiptDetailId", "dbo.CountingReceiptDetail");
            DropIndex("dbo.CountingReceiptSerial", new[] { "CountingReceiptDetailId" });
            DropTable("dbo.CountingReceiptSerial");
        }
    }
}
