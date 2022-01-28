namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateCityAndCountryAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "CityShipperId", c => c.Int());
            AddColumn("dbo.ItemLoad", "CityBuyerId", c => c.Int());
            AddColumn("dbo.ItemLoad", "CountryShipperId", c => c.Int());
            AddColumn("dbo.ItemLoad", "CountryBuyerId", c => c.Int());
            CreateIndex("dbo.ItemLoad", "CityShipperId");
            CreateIndex("dbo.ItemLoad", "CityBuyerId");
            CreateIndex("dbo.ItemLoad", "CountryShipperId");
            CreateIndex("dbo.ItemLoad", "CountryBuyerId");
            AddForeignKey("dbo.ItemLoad", "CountryBuyerId", "dbo.Country", "Id");
            AddForeignKey("dbo.ItemLoad", "CountryShipperId", "dbo.Country", "Id");
            AddForeignKey("dbo.ItemLoad", "CityBuyerId", "dbo.City", "Id");
            AddForeignKey("dbo.ItemLoad", "CityShipperId", "dbo.City", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemLoad", "CityShipperId", "dbo.City");
            DropForeignKey("dbo.ItemLoad", "CityBuyerId", "dbo.City");
            DropForeignKey("dbo.ItemLoad", "CountryShipperId", "dbo.Country");
            DropForeignKey("dbo.ItemLoad", "CountryBuyerId", "dbo.Country");
            DropIndex("dbo.ItemLoad", new[] { "CountryBuyerId" });
            DropIndex("dbo.ItemLoad", new[] { "CountryShipperId" });
            DropIndex("dbo.ItemLoad", new[] { "CityBuyerId" });
            DropIndex("dbo.ItemLoad", new[] { "CityShipperId" });
            DropColumn("dbo.ItemLoad", "CountryBuyerId");
            DropColumn("dbo.ItemLoad", "CountryShipperId");
            DropColumn("dbo.ItemLoad", "CityBuyerId");
            DropColumn("dbo.ItemLoad", "CityShipperId");
        }
    }
}
