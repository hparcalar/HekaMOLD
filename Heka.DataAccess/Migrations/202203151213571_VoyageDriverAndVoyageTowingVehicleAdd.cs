namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageDriverAndVoyageTowingVehicleAdd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VoyageDriver",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DriverId = c.Int(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        StartKmHour = c.Int(),
                        EndKmHour = c.Int(),
                        TowinfVehicleId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Vehicle", t => t.TowinfVehicleId)
                .ForeignKey("dbo.Driver", t => t.DriverId)
                .Index(t => t.DriverId)
                .Index(t => t.TowinfVehicleId);
            
            CreateTable(
                "dbo.VoyageTowingVehicle",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TowingVehicleId = c.Int(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        StartKmHour = c.Int(),
                        EndKmHour = c.Int(),
                        DriverId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Vehicle", t => t.TowingVehicleId)
                .ForeignKey("dbo.Driver", t => t.DriverId)
                .Index(t => t.TowingVehicleId)
                .Index(t => t.DriverId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VoyageTowingVehicle", "DriverId", "dbo.Driver");
            DropForeignKey("dbo.VoyageDriver", "DriverId", "dbo.Driver");
            DropForeignKey("dbo.VoyageTowingVehicle", "TowingVehicleId", "dbo.Vehicle");
            DropForeignKey("dbo.VoyageDriver", "TowinfVehicleId", "dbo.Vehicle");
            DropIndex("dbo.VoyageTowingVehicle", new[] { "DriverId" });
            DropIndex("dbo.VoyageTowingVehicle", new[] { "TowingVehicleId" });
            DropIndex("dbo.VoyageDriver", new[] { "TowinfVehicleId" });
            DropIndex("dbo.VoyageDriver", new[] { "DriverId" });
            DropTable("dbo.VoyageTowingVehicle");
            DropTable("dbo.VoyageDriver");
        }
    }
}
