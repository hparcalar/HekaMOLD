namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateCityAndCountryAdd2 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.ItemLoad", name: "CityBuyerId", newName: "BuyerCityId");
            RenameColumn(table: "dbo.ItemLoad", name: "CityShipperId", newName: "ShipperCityId");
            RenameColumn(table: "dbo.ItemLoad", name: "CountryBuyerId", newName: "BuyerCountryId");
            RenameColumn(table: "dbo.ItemLoad", name: "CountryShipperId", newName: "ShipperCountryId");
            RenameIndex(table: "dbo.ItemLoad", name: "IX_CityShipperId", newName: "IX_ShipperCityId");
            RenameIndex(table: "dbo.ItemLoad", name: "IX_CityBuyerId", newName: "IX_BuyerCityId");
            RenameIndex(table: "dbo.ItemLoad", name: "IX_CountryShipperId", newName: "IX_ShipperCountryId");
            RenameIndex(table: "dbo.ItemLoad", name: "IX_CountryBuyerId", newName: "IX_BuyerCountryId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.ItemLoad", name: "IX_BuyerCountryId", newName: "IX_CountryBuyerId");
            RenameIndex(table: "dbo.ItemLoad", name: "IX_ShipperCountryId", newName: "IX_CountryShipperId");
            RenameIndex(table: "dbo.ItemLoad", name: "IX_BuyerCityId", newName: "IX_CityBuyerId");
            RenameIndex(table: "dbo.ItemLoad", name: "IX_ShipperCityId", newName: "IX_CityShipperId");
            RenameColumn(table: "dbo.ItemLoad", name: "ShipperCountryId", newName: "CountryShipperId");
            RenameColumn(table: "dbo.ItemLoad", name: "BuyerCountryId", newName: "CountryBuyerId");
            RenameColumn(table: "dbo.ItemLoad", name: "ShipperCityId", newName: "CityShipperId");
            RenameColumn(table: "dbo.ItemLoad", name: "BuyerCityId", newName: "CityBuyerId");
        }
    }
}
