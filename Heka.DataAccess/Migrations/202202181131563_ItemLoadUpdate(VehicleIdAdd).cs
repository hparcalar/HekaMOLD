namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateVehicleIdAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "VehicleId", c => c.Int());
            CreateIndex("dbo.ItemLoad", "VehicleId");
            AddForeignKey("dbo.ItemLoad", "VehicleId", "dbo.Vehicle", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemLoad", "VehicleId", "dbo.Vehicle");
            DropIndex("dbo.ItemLoad", new[] { "VehicleId" });
            DropColumn("dbo.ItemLoad", "VehicleId");
        }
    }
}
