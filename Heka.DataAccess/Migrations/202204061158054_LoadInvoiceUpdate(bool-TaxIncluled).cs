namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LoadInvoiceUpdateboolTaxIncluled : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.LoadInvoice", "TaxIncluded", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.LoadInvoice", "TaxIncluded", c => c.Byte(nullable: false));
        }
    }
}
