namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateTowinfVehicleAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "TowinfVehicleId", c => c.Int());
            CreateIndex("dbo.ItemLoad", "TowinfVehicleId");
            AddForeignKey("dbo.ItemLoad", "TowinfVehicleId", "dbo.Vehicle", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemLoad", "TowinfVehicleId", "dbo.Vehicle");
            DropIndex("dbo.ItemLoad", new[] { "TowinfVehicleId" });
            DropColumn("dbo.ItemLoad", "TowinfVehicleId");
        }
    }
}
