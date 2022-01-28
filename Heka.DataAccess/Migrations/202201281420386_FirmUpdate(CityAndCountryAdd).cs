namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirmUpdateCityAndCountryAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Firm", "CityId", c => c.Int());
            AddColumn("dbo.Firm", "CountryId", c => c.Int());
            CreateIndex("dbo.Firm", "CityId");
            CreateIndex("dbo.Firm", "CountryId");
            AddForeignKey("dbo.Firm", "CountryId", "dbo.Country", "Id");
            AddForeignKey("dbo.Firm", "CityId", "dbo.City", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Firm", "CityId", "dbo.City");
            DropForeignKey("dbo.Firm", "CountryId", "dbo.Country");
            DropIndex("dbo.Firm", new[] { "CountryId" });
            DropIndex("dbo.Firm", new[] { "CityId" });
            DropColumn("dbo.Firm", "CountryId");
            DropColumn("dbo.Firm", "CityId");
        }
    }
}
