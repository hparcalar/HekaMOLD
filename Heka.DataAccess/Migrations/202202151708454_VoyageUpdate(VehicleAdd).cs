namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageUpdateVehicleAdd : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Voyage", "CarrierFirmId");
            CreateIndex("dbo.Voyage", "DriverId");
            CreateIndex("dbo.Voyage", "TowinfVehicleId");
            CreateIndex("dbo.Voyage", "TraillerVehicleId");
            AddForeignKey("dbo.Voyage", "TowinfVehicleId", "dbo.Vehicle", "Id");
            AddForeignKey("dbo.Voyage", "TraillerVehicleId", "dbo.Vehicle", "Id");
            AddForeignKey("dbo.Voyage", "DriverId", "dbo.Driver", "Id");
            AddForeignKey("dbo.Voyage", "CarrierFirmId", "dbo.Firm", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Voyage", "CarrierFirmId", "dbo.Firm");
            DropForeignKey("dbo.Voyage", "DriverId", "dbo.Driver");
            DropForeignKey("dbo.Voyage", "TraillerVehicleId", "dbo.Vehicle");
            DropForeignKey("dbo.Voyage", "TowinfVehicleId", "dbo.Vehicle");
            DropIndex("dbo.Voyage", new[] { "TraillerVehicleId" });
            DropIndex("dbo.Voyage", new[] { "TowinfVehicleId" });
            DropIndex("dbo.Voyage", new[] { "DriverId" });
            DropIndex("dbo.Voyage", new[] { "CarrierFirmId" });
        }
    }
}
