namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageDriverUpdateVoyageIdAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VoyageDriver", "VoyageId", c => c.Int());
            AddColumn("dbo.VoyageTowingVehicle", "VoyageId", c => c.Int());
            CreateIndex("dbo.VoyageDriver", "VoyageId");
            CreateIndex("dbo.VoyageTowingVehicle", "VoyageId");
            AddForeignKey("dbo.VoyageDriver", "VoyageId", "dbo.Voyage", "Id");
            AddForeignKey("dbo.VoyageTowingVehicle", "VoyageId", "dbo.Voyage", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VoyageTowingVehicle", "VoyageId", "dbo.Voyage");
            DropForeignKey("dbo.VoyageDriver", "VoyageId", "dbo.Voyage");
            DropIndex("dbo.VoyageTowingVehicle", new[] { "VoyageId" });
            DropIndex("dbo.VoyageDriver", new[] { "VoyageId" });
            DropColumn("dbo.VoyageTowingVehicle", "VoyageId");
            DropColumn("dbo.VoyageDriver", "VoyageId");
        }
    }
}
