namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateVoyageStartCountryAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "VoyageStartAddress", c => c.String());
            AddColumn("dbo.ItemLoad", "VoyageEndAddress", c => c.String());
            AddColumn("dbo.ItemLoad", "LoadingLineNo", c => c.Int());
            AddColumn("dbo.ItemLoad", "DischargeLineNo", c => c.Int());
            AddColumn("dbo.ItemLoad", "KapikuleEntryDate", c => c.DateTime());
            AddColumn("dbo.ItemLoad", "KapikuleExitDate", c => c.DateTime());
            AddColumn("dbo.ItemLoad", "VoyageStartCountryId", c => c.Int());
            AddColumn("dbo.ItemLoad", "VoyageEndCountryId", c => c.Int());
            CreateIndex("dbo.ItemLoad", "VoyageStartCountryId");
            CreateIndex("dbo.ItemLoad", "VoyageEndCountryId");
            AddForeignKey("dbo.ItemLoad", "VoyageEndCountryId", "dbo.Country", "Id");
            AddForeignKey("dbo.ItemLoad", "VoyageStartCountryId", "dbo.Country", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemLoad", "VoyageStartCountryId", "dbo.Country");
            DropForeignKey("dbo.ItemLoad", "VoyageEndCountryId", "dbo.Country");
            DropIndex("dbo.ItemLoad", new[] { "VoyageEndCountryId" });
            DropIndex("dbo.ItemLoad", new[] { "VoyageStartCountryId" });
            DropColumn("dbo.ItemLoad", "VoyageEndCountryId");
            DropColumn("dbo.ItemLoad", "VoyageStartCountryId");
            DropColumn("dbo.ItemLoad", "KapikuleExitDate");
            DropColumn("dbo.ItemLoad", "KapikuleEntryDate");
            DropColumn("dbo.ItemLoad", "DischargeLineNo");
            DropColumn("dbo.ItemLoad", "LoadingLineNo");
            DropColumn("dbo.ItemLoad", "VoyageEndAddress");
            DropColumn("dbo.ItemLoad", "VoyageStartAddress");
        }
    }
}
