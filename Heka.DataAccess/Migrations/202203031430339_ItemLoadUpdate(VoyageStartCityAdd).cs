namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateVoyageStartCityAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "VoyageExitDate", c => c.DateTime());
            AddColumn("dbo.ItemLoad", "VoyageEndDate", c => c.DateTime());
            AddColumn("dbo.ItemLoad", "VoyageStartCityId", c => c.Int());
            AddColumn("dbo.ItemLoad", "VoyageEndCityId", c => c.Int());
            CreateIndex("dbo.ItemLoad", "VoyageStartCityId");
            CreateIndex("dbo.ItemLoad", "VoyageEndCityId");
            AddForeignKey("dbo.ItemLoad", "VoyageEndCityId", "dbo.City", "Id");
            AddForeignKey("dbo.ItemLoad", "VoyageStartCityId", "dbo.City", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemLoad", "VoyageStartCityId", "dbo.City");
            DropForeignKey("dbo.ItemLoad", "VoyageEndCityId", "dbo.City");
            DropIndex("dbo.ItemLoad", new[] { "VoyageEndCityId" });
            DropIndex("dbo.ItemLoad", new[] { "VoyageStartCityId" });
            DropColumn("dbo.ItemLoad", "VoyageEndCityId");
            DropColumn("dbo.ItemLoad", "VoyageStartCityId");
            DropColumn("dbo.ItemLoad", "VoyageEndDate");
            DropColumn("dbo.ItemLoad", "VoyageExitDate");
        }
    }
}
