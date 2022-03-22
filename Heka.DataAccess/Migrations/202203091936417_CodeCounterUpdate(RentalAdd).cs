namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CodeCounterUpdateRentalAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CodeCounter", "OwnExport", c => c.Int());
            AddColumn("dbo.CodeCounter", "RentalExport", c => c.Int());
            AddColumn("dbo.CodeCounter", "OwnImport", c => c.Int());
            AddColumn("dbo.CodeCounter", "RentalImport", c => c.Int());
            AddColumn("dbo.CodeCounter", "OwnDomestic", c => c.Int());
            AddColumn("dbo.CodeCounter", "RentalDomestic", c => c.Int());
            AddColumn("dbo.CodeCounter", "OwnTransit", c => c.Int());
            AddColumn("dbo.CodeCounter", "RentalTransit", c => c.Int());
            DropColumn("dbo.CodeCounter", "Export");
            DropColumn("dbo.CodeCounter", "Import");
            DropColumn("dbo.CodeCounter", "Domestic");
            DropColumn("dbo.CodeCounter", "Transit");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CodeCounter", "Transit", c => c.Int());
            AddColumn("dbo.CodeCounter", "Domestic", c => c.Int());
            AddColumn("dbo.CodeCounter", "Import", c => c.Int());
            AddColumn("dbo.CodeCounter", "Export", c => c.Int());
            DropColumn("dbo.CodeCounter", "RentalTransit");
            DropColumn("dbo.CodeCounter", "OwnTransit");
            DropColumn("dbo.CodeCounter", "RentalDomestic");
            DropColumn("dbo.CodeCounter", "OwnDomestic");
            DropColumn("dbo.CodeCounter", "RentalImport");
            DropColumn("dbo.CodeCounter", "OwnImport");
            DropColumn("dbo.CodeCounter", "RentalExport");
            DropColumn("dbo.CodeCounter", "OwnExport");
        }
    }
}
