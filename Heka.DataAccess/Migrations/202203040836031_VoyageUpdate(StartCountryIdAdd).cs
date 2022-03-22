namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageUpdateStartCountryIdAdd : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Voyage", new[] { "entryCustomsId" });
            AddColumn("dbo.Voyage", "FirstLoadDate", c => c.DateTime());
            AddColumn("dbo.Voyage", "EndDischargeDate", c => c.DateTime());
            AddColumn("dbo.Voyage", "KapikulePassportEntryDate", c => c.DateTime());
            AddColumn("dbo.Voyage", "KapikulePassportExitDate", c => c.DateTime());
            AddColumn("dbo.Voyage", "StartCountryId", c => c.Int());
            CreateIndex("dbo.Voyage", "StartCountryId");
            CreateIndex("dbo.Voyage", "EntryCustomsId");
            AddForeignKey("dbo.Voyage", "StartCountryId", "dbo.Country", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Voyage", "StartCountryId", "dbo.Country");
            DropIndex("dbo.Voyage", new[] { "EntryCustomsId" });
            DropIndex("dbo.Voyage", new[] { "StartCountryId" });
            DropColumn("dbo.Voyage", "StartCountryId");
            DropColumn("dbo.Voyage", "KapikulePassportExitDate");
            DropColumn("dbo.Voyage", "KapikulePassportEntryDate");
            DropColumn("dbo.Voyage", "EndDischargeDate");
            DropColumn("dbo.Voyage", "FirstLoadDate");
            CreateIndex("dbo.Voyage", "entryCustomsId");
        }
    }
}
