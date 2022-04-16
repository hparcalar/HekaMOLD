namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LoadInvoiceAdd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LoadInvoice",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InvoiceType = c.Int(nullable: false),
                        FirmId = c.Int(nullable: false),
                        ItemLoadId = c.Int(nullable: false),
                        ServiceItemId = c.Int(nullable: false),
                        InvoiceDate = c.DateTime(),
                        OverallTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxRate = c.Int(nullable: false),
                        TaxIncluded = c.Byte(nullable: false),
                        ForexRate = c.Int(nullable: false),
                        ForexId = c.Int(nullable: false),
                        Explanation = c.String(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ServiceItem", t => t.ServiceItemId, cascadeDelete: true)
                .ForeignKey("dbo.ItemLoad", t => t.ItemLoadId, cascadeDelete: true)
                .ForeignKey("dbo.ForexType", t => t.ForexId, cascadeDelete: true)
                .ForeignKey("dbo.Firm", t => t.FirmId, cascadeDelete: true)
                .Index(t => t.FirmId)
                .Index(t => t.ItemLoadId)
                .Index(t => t.ServiceItemId)
                .Index(t => t.ForexId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LoadInvoice", "FirmId", "dbo.Firm");
            DropForeignKey("dbo.LoadInvoice", "ForexId", "dbo.ForexType");
            DropForeignKey("dbo.LoadInvoice", "ItemLoadId", "dbo.ItemLoad");
            DropForeignKey("dbo.LoadInvoice", "ServiceItemId", "dbo.ServiceItem");
            DropIndex("dbo.LoadInvoice", new[] { "ForexId" });
            DropIndex("dbo.LoadInvoice", new[] { "ServiceItemId" });
            DropIndex("dbo.LoadInvoice", new[] { "ItemLoadId" });
            DropIndex("dbo.LoadInvoice", new[] { "FirmId" });
            DropTable("dbo.LoadInvoice");
        }
    }
}
