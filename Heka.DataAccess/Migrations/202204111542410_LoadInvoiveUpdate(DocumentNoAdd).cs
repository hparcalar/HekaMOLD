namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LoadInvoiveUpdateDocumentNoAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LoadInvoice", "DocumentNo", c => c.String());
            AddColumn("dbo.LoadInvoice", "IntegrationId", c => c.Int());
            AddColumn("dbo.LoadInvoice", "Integration", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LoadInvoice", "Integration");
            DropColumn("dbo.LoadInvoice", "IntegrationId");
            DropColumn("dbo.LoadInvoice", "DocumentNo");
        }
    }
}
