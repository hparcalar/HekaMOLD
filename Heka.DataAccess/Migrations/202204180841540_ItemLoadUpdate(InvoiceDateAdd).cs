namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateInvoiceDateAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "Billed", c => c.Boolean());
            AddColumn("dbo.ItemLoad", "InvoiceDocumentNo", c => c.String());
            AddColumn("dbo.ItemLoad", "InvoiceDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemLoad", "InvoiceDate");
            DropColumn("dbo.ItemLoad", "InvoiceDocumentNo");
            DropColumn("dbo.ItemLoad", "Billed");
        }
    }
}
