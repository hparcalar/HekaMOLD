namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageAdd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Voyage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VoyageCode = c.String(),
                        VoyageDate = c.DateTime(),
                        DriverId = c.Int(),
                        TowinfVehicleId = c.Int(),
                        TraillerVehicleId = c.Int(),
                        VoyageStatus = c.Int(),
                        StartDate = c.DateTime(),
                        LoadDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        CarrierFirmId = c.Int(),
                        TraillerType = c.Int(),
                        UploadType = c.Int(),
                        HasTraillerTransportation = c.Boolean(),
                        EmptyGo = c.Boolean(),
                        HasNotRationCard = c.Boolean(),
                        CustomsDoorEntryId = c.Int(),
                        CustomsDoorExitId = c.Int(),
                        CustomsDoorEntryDate = c.DateTime(),
                        CustomsDoorExitDate = c.DateTime(),
                        TraillerRationCardNo = c.String(),
                        TraillerRationCardClosedDate = c.DateTime(),
                        OverallQuantity = c.Decimal(precision: 18, scale: 2),
                        OverallVolume = c.Decimal(precision: 18, scale: 2),
                        OverallGrossWeight = c.Decimal(precision: 18, scale: 2),
                        OverallLadametre = c.Decimal(precision: 18, scale: 2),
                        LoadCustomsId = c.Int(),
                        DischargeCustomsId = c.Int(),
                        PositionKmHour = c.Int(),
                        LoadImportantType = c.Int(),
                        RingCode = c.String(),
                        DocumentNo = c.String(),
                        HasOperation = c.Boolean(),
                        LoadCityId = c.Int(),
                        LoadCountryId = c.Int(),
                        LoadAddress = c.String(),
                        DischargeCityId = c.Int(),
                        DischargeCountryId = c.Int(),
                        DischargeAddress = c.String(),
                        ClosedDate = c.DateTime(),
                        VehicleExitDate = c.DateTime(),
                        RouteKmHour = c.Int(),
                        RouteDefinition = c.String(),
                        EndTime = c.DateTime(),
                        Explanation = c.String(),
                        CreatedUserId = c.Int(),
                        StartCityId = c.Int(),
                        EndCityId = c.Int(),
                        PlantId = c.Int(),
                        SyncStatus = c.Int(),
                        SyncPointId = c.Int(),
                        SyncDate = c.DateTime(),
                        SyncUserId = c.Int(),
                        CreatedDate = c.DateTime(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                        SyncKey = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.CreatedUserId)
                .ForeignKey("dbo.City", t => t.EndCityId)
                .ForeignKey("dbo.City", t => t.StartCityId)
                .Index(t => t.CreatedUserId)
                .Index(t => t.StartCityId)
                .Index(t => t.EndCityId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Voyage", "StartCityId", "dbo.City");
            DropForeignKey("dbo.Voyage", "EndCityId", "dbo.City");
            DropForeignKey("dbo.Voyage", "CreatedUserId", "dbo.User");
            DropIndex("dbo.Voyage", new[] { "EndCityId" });
            DropIndex("dbo.Voyage", new[] { "StartCityId" });
            DropIndex("dbo.Voyage", new[] { "CreatedUserId" });
            DropTable("dbo.Voyage");
        }
    }
}
