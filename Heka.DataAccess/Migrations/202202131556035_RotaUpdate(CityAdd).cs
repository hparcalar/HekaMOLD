namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RotaUpdateCityAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Rota", "CityStartId", c => c.Int());
            AddColumn("dbo.Rota", "CityEndId", c => c.Int());
            CreateIndex("dbo.Rota", "CityStartId");
            CreateIndex("dbo.Rota", "CityEndId");
            AddForeignKey("dbo.Rota", "CityEndId", "dbo.City", "Id");
            AddForeignKey("dbo.Rota", "CityStartId", "dbo.City", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Rota", "CityStartId", "dbo.City");
            DropForeignKey("dbo.Rota", "CityEndId", "dbo.City");
            DropIndex("dbo.Rota", new[] { "CityEndId" });
            DropIndex("dbo.Rota", new[] { "CityStartId" });
            DropColumn("dbo.Rota", "CityEndId");
            DropColumn("dbo.Rota", "CityStartId");
        }
    }
}
