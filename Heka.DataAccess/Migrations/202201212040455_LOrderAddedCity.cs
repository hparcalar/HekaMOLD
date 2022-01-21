namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LOrderAddedCity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemOrder", "LoadCityId", c => c.Int());
            AddColumn("dbo.ItemOrder", "DischargeCityId", c => c.Int());
            CreateIndex("dbo.ItemOrder", "LoadCityId");
            CreateIndex("dbo.ItemOrder", "DischargeCityId");
            AddForeignKey("dbo.ItemOrder", "DischargeCityId", "dbo.City", "Id");
            AddForeignKey("dbo.ItemOrder", "LoadCityId", "dbo.City", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemOrder", "LoadCityId", "dbo.City");
            DropForeignKey("dbo.ItemOrder", "DischargeCityId", "dbo.City");
            DropIndex("dbo.ItemOrder", new[] { "DischargeCityId" });
            DropIndex("dbo.ItemOrder", new[] { "LoadCityId" });
            DropColumn("dbo.ItemOrder", "DischargeCityId");
            DropColumn("dbo.ItemOrder", "LoadCityId");
        }
    }
}
