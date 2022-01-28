namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateLoadAuthorAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "LoadAuthor", c => c.String());
            AddColumn("dbo.ItemLoad", "LoadingDate", c => c.DateTime());
            AddColumn("dbo.ItemLoad", "InvoiceId", c => c.Int());
            AddColumn("dbo.ItemLoad", "InvoiceStatus", c => c.Int());
            AddColumn("dbo.ItemLoad", "InvoiceFreightPrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemLoad", "CrmNo", c => c.String());
            AddColumn("dbo.ItemLoad", "CrmStatus", c => c.Int());
            CreateIndex("dbo.ItemLoad", "InvoiceId");
            AddForeignKey("dbo.ItemLoad", "InvoiceId", "dbo.Invoice", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemLoad", "InvoiceId", "dbo.Invoice");
            DropIndex("dbo.ItemLoad", new[] { "InvoiceId" });
            DropColumn("dbo.ItemLoad", "CrmStatus");
            DropColumn("dbo.ItemLoad", "CrmNo");
            DropColumn("dbo.ItemLoad", "InvoiceFreightPrice");
            DropColumn("dbo.ItemLoad", "InvoiceStatus");
            DropColumn("dbo.ItemLoad", "InvoiceId");
            DropColumn("dbo.ItemLoad", "LoadingDate");
            DropColumn("dbo.ItemLoad", "LoadAuthor");
        }
    }
}
