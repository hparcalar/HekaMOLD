namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateVoyageCreatedUserAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "VoyageCode", c => c.String());
            AddColumn("dbo.ItemLoad", "DriverId", c => c.Int());
            AddColumn("dbo.ItemLoad", "VoyageCreatedUserId", c => c.Int());
            CreateIndex("dbo.ItemLoad", "DriverId");
            CreateIndex("dbo.ItemLoad", "VoyageCreatedUserId");
            AddForeignKey("dbo.ItemLoad", "VoyageCreatedUserId", "dbo.User", "Id");
            AddForeignKey("dbo.ItemLoad", "DriverId", "dbo.Driver", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemLoad", "DriverId", "dbo.Driver");
            DropForeignKey("dbo.ItemLoad", "VoyageCreatedUserId", "dbo.User");
            DropIndex("dbo.ItemLoad", new[] { "VoyageCreatedUserId" });
            DropIndex("dbo.ItemLoad", new[] { "DriverId" });
            DropColumn("dbo.ItemLoad", "VoyageCreatedUserId");
            DropColumn("dbo.ItemLoad", "DriverId");
            DropColumn("dbo.ItemLoad", "VoyageCode");
        }
    }
}
