namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Vehicle : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Vehicle",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Plate = c.String(),
                        Mark = c.String(),
                        Model = c.String(),
                        ChassisNumber = c.String(),
                        VehicleAllocationType = c.Int(),
                        VehicleTypeId = c.Int(),
                        OwnerFirmId = c.Int(),
                        ContractStartDate = c.DateTime(),
                        ContractEndDate = c.DateTime(),
                        KmHour = c.Int(),
                        Amount = c.Decimal(precision: 18, scale: 2),
                        UnitId = c.Int(),
                        Length = c.Decimal(precision: 18, scale: 2),
                        Width = c.Decimal(precision: 18, scale: 2),
                        Height = c.Decimal(precision: 18, scale: 2),
                        TrailerHeadWeight = c.Decimal(precision: 18, scale: 2),
                        LoadCapacity = c.Decimal(precision: 18, scale: 2),
                        TrailerType = c.Int(),
                        StatusCode = c.Boolean(),
                        CarePeriyot = c.Int(),
                        ProportionalLimit = c.Int(),
                        CareNotification = c.Boolean(),
                        TireNotification = c.Boolean(),
                        Approval = c.Boolean(),
                        Invalidation = c.Boolean(),
                        KmHourControl = c.Boolean(),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Firm", t => t.OwnerFirmId)
                .ForeignKey("dbo.UnitType", t => t.UnitId)
                .ForeignKey("dbo.VehicleType", t => t.VehicleTypeId)
                .Index(t => t.VehicleTypeId)
                .Index(t => t.OwnerFirmId)
                .Index(t => t.UnitId);
            
            CreateTable(
                "dbo.VehicleCare",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VehiceTireType = c.Int(),
                        SeriNo = c.String(),
                        DirectionType = c.Int(),
                        DimensionsInfo = c.String(),
                        MontageDate = c.DateTime(),
                        VehicleCareTypeId = c.Int(),
                        VehicleId = c.Int(),
                        OperationFirmId = c.Int(),
                        KmHour = c.Int(),
                        KmHourLimit = c.Int(),
                        KmHourControl = c.Boolean(),
                        UnitId = c.Int(),
                        Invalidation = c.Boolean(),
                        Amount = c.Decimal(precision: 18, scale: 2),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Firm", t => t.OperationFirmId)
                .ForeignKey("dbo.UnitType", t => t.UnitId)
                .ForeignKey("dbo.VehicleCareType", t => t.VehicleCareTypeId)
                .ForeignKey("dbo.Vehicle", t => t.VehicleId)
                .Index(t => t.VehicleCareTypeId)
                .Index(t => t.VehicleId)
                .Index(t => t.OperationFirmId)
                .Index(t => t.UnitId);
            
            CreateTable(
                "dbo.VehicleCareType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VehicleCareTypeCode = c.String(),
                        VehicleCareTypeName = c.String(),
                        PeriyodUnitCode = c.Int(),
                        Compulsory = c.Boolean(),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.VehicleInsurance",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VehicleInsuranceTypeId = c.Int(),
                        VehicleId = c.Int(),
                        OperationFirmId = c.Int(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        KmHour = c.Int(),
                        PersonnelId = c.Int(),
                        UnitId = c.Int(),
                        Amount = c.Decimal(precision: 18, scale: 2),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Firm", t => t.OperationFirmId)
                .ForeignKey("dbo.UnitType", t => t.UnitId)
                .ForeignKey("dbo.VehicleInsuranceType", t => t.VehicleInsuranceTypeId)
                .ForeignKey("dbo.Vehicle", t => t.VehicleId)
                .Index(t => t.VehicleInsuranceTypeId)
                .Index(t => t.VehicleId)
                .Index(t => t.OperationFirmId)
                .Index(t => t.UnitId);
            
            CreateTable(
                "dbo.VehicleInsuranceType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VehicleInsuranceTypeCode = c.String(),
                        VehicleInsuranceTypeName = c.String(),
                        PeriyodUnitid = c.Int(),
                        Compulsory = c.Boolean(),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UnitType", t => t.PeriyodUnitid)
                .Index(t => t.PeriyodUnitid);
            
            CreateTable(
                "dbo.VehicleTire",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VehicleTireTyp = c.Int(),
                        SeriNo = c.String(),
                        DirectionType = c.Int(),
                        DimensionsInfo = c.String(),
                        MontageDate = c.DateTime(),
                        VehicleId = c.Int(),
                        OperationFirmId = c.Int(),
                        KmHour = c.Int(),
                        KmHourLimit = c.Int(),
                        KmHourControl = c.Boolean(),
                        UnitId = c.Int(),
                        Invalidation = c.Boolean(),
                        Amount = c.Decimal(precision: 18, scale: 2),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Firm", t => t.OperationFirmId)
                .ForeignKey("dbo.UnitType", t => t.UnitId)
                .ForeignKey("dbo.Vehicle", t => t.VehicleId)
                .Index(t => t.VehicleId)
                .Index(t => t.OperationFirmId)
                .Index(t => t.UnitId);
            
            CreateTable(
                "dbo.VehicleType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VehicleTypeCode = c.String(),
                        VehicleTypeName = c.String(),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.VehicleNotification",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CareStep = c.Int(),
                        MailNotification = c.Boolean(),
                        SmsNotification = c.Boolean(),
                        ExtraNotification = c.Boolean(),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Vehicle", "VehicleTypeId", "dbo.VehicleType");
            DropForeignKey("dbo.VehicleTire", "VehicleId", "dbo.Vehicle");
            DropForeignKey("dbo.VehicleTire", "UnitId", "dbo.UnitType");
            DropForeignKey("dbo.VehicleTire", "OperationFirmId", "dbo.Firm");
            DropForeignKey("dbo.VehicleInsurance", "VehicleId", "dbo.Vehicle");
            DropForeignKey("dbo.VehicleInsurance", "VehicleInsuranceTypeId", "dbo.VehicleInsuranceType");
            DropForeignKey("dbo.VehicleInsuranceType", "PeriyodUnitid", "dbo.UnitType");
            DropForeignKey("dbo.VehicleInsurance", "UnitId", "dbo.UnitType");
            DropForeignKey("dbo.VehicleInsurance", "OperationFirmId", "dbo.Firm");
            DropForeignKey("dbo.VehicleCare", "VehicleId", "dbo.Vehicle");
            DropForeignKey("dbo.VehicleCare", "VehicleCareTypeId", "dbo.VehicleCareType");
            DropForeignKey("dbo.VehicleCare", "UnitId", "dbo.UnitType");
            DropForeignKey("dbo.VehicleCare", "OperationFirmId", "dbo.Firm");
            DropForeignKey("dbo.Vehicle", "UnitId", "dbo.UnitType");
            DropForeignKey("dbo.Vehicle", "OwnerFirmId", "dbo.Firm");
            DropIndex("dbo.VehicleTire", new[] { "UnitId" });
            DropIndex("dbo.VehicleTire", new[] { "OperationFirmId" });
            DropIndex("dbo.VehicleTire", new[] { "VehicleId" });
            DropIndex("dbo.VehicleInsuranceType", new[] { "PeriyodUnitid" });
            DropIndex("dbo.VehicleInsurance", new[] { "UnitId" });
            DropIndex("dbo.VehicleInsurance", new[] { "OperationFirmId" });
            DropIndex("dbo.VehicleInsurance", new[] { "VehicleId" });
            DropIndex("dbo.VehicleInsurance", new[] { "VehicleInsuranceTypeId" });
            DropIndex("dbo.VehicleCare", new[] { "UnitId" });
            DropIndex("dbo.VehicleCare", new[] { "OperationFirmId" });
            DropIndex("dbo.VehicleCare", new[] { "VehicleId" });
            DropIndex("dbo.VehicleCare", new[] { "VehicleCareTypeId" });
            DropIndex("dbo.Vehicle", new[] { "UnitId" });
            DropIndex("dbo.Vehicle", new[] { "OwnerFirmId" });
            DropIndex("dbo.Vehicle", new[] { "VehicleTypeId" });
            DropTable("dbo.VehicleNotification");
            DropTable("dbo.VehicleType");
            DropTable("dbo.VehicleTire");
            DropTable("dbo.VehicleInsuranceType");
            DropTable("dbo.VehicleInsurance");
            DropTable("dbo.VehicleCareType");
            DropTable("dbo.VehicleCare");
            DropTable("dbo.Vehicle");
        }
    }
}
