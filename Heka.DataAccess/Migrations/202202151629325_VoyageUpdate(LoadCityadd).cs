namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageUpdateLoadCityadd : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Voyage", "LoadCityId");
            CreateIndex("dbo.Voyage", "DischargeCityId");
            AddForeignKey("dbo.Voyage", "DischargeCityId", "dbo.City", "Id");
            AddForeignKey("dbo.Voyage", "LoadCityId", "dbo.City", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Voyage", "LoadCityId", "dbo.City");
            DropForeignKey("dbo.Voyage", "DischargeCityId", "dbo.City");
            DropIndex("dbo.Voyage", new[] { "DischargeCityId" });
            DropIndex("dbo.Voyage", new[] { "LoadCityId" });
        }
    }
}
