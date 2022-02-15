namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageUpdateLoadCountryAdd : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Voyage", "DischargeCountryId");
            CreateIndex("dbo.Voyage", "LoadCountryId");
            AddForeignKey("dbo.Voyage", "DischargeCountryId", "dbo.Country", "Id");
            AddForeignKey("dbo.Voyage", "LoadCountryId", "dbo.Country", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Voyage", "LoadCountryId", "dbo.Country");
            DropForeignKey("dbo.Voyage", "DischargeCountryId", "dbo.Country");
            DropIndex("dbo.Voyage", new[] { "LoadCountryId" });
            DropIndex("dbo.Voyage", new[] { "DischargeCountryId" });
        }
    }
}
