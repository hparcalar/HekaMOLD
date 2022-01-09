namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class vehiclelmqwefwee : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FabricRecipe", "FirmId", "dbo.Firm");
            DropForeignKey("dbo.FabricRecipe", "ItemId", "dbo.Item");
            DropForeignKey("dbo.KnitYarn", "FirmId", "dbo.Firm");
            DropForeignKey("dbo.KnitYarn", "YarnRecipeId", "dbo.YarnRecipe");
            DropForeignKey("dbo.YarnRecipe", "YarnBreedId", "dbo.YarnBreed");
            DropForeignKey("dbo.YarnRecipeMix", "YarnBreedId", "dbo.YarnBreed");
            DropForeignKey("dbo.YarnColour", "YarnColourGroupId", "dbo.YarnColourGroup");
            DropForeignKey("dbo.YarnRecipe", "YarnColourId", "dbo.YarnColour");
            DropForeignKey("dbo.YarnRecipeMix", "YarnRecipeId", "dbo.YarnRecipe");
            DropForeignKey("dbo.KnitYarn", "ItemId", "dbo.Item");
            DropForeignKey("dbo.Item", "MachineId", "dbo.Machine");
            DropIndex("dbo.Item", new[] { "MachineId" });
            DropIndex("dbo.FabricRecipe", new[] { "ItemId" });
            DropIndex("dbo.FabricRecipe", new[] { "FirmId" });
            DropIndex("dbo.KnitYarn", new[] { "YarnRecipeId" });
            DropIndex("dbo.KnitYarn", new[] { "FirmId" });
            DropIndex("dbo.KnitYarn", new[] { "ItemId" });
            DropIndex("dbo.YarnRecipe", new[] { "YarnBreedId" });
            DropIndex("dbo.YarnRecipe", new[] { "YarnColourId" });
            DropIndex("dbo.YarnRecipeMix", new[] { "YarnRecipeId" });
            DropIndex("dbo.YarnRecipeMix", new[] { "YarnBreedId" });
            DropIndex("dbo.YarnColour", new[] { "YarnColourGroupId" });
            CreateTable(
                "dbo.YarnRecipeType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        YarnTypeCode = c.String(),
                        YarnTypeName = c.String(),
                        IsActive = c.Boolean(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
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
                        Price = c.Decimal(precision: 18, scale: 2),
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
            
            AddColumn("dbo.ItemKnitDensity", "YarnRecipeTypeId", c => c.Int(nullable: false));
            AddColumn("dbo.ItemQualityType", "IsActive", c => c.Boolean());
            AddColumn("dbo.ItemQualityType", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.ItemQualityType", "CreatedUserId", c => c.Int());
            AddColumn("dbo.ItemQualityType", "UpdatedDate", c => c.DateTime());
            AddColumn("dbo.ItemQualityType", "UpdatedUserId", c => c.Int());
            AddColumn("dbo.YarnRecipe", "ItemId", c => c.Int());
            AddColumn("dbo.YarnRecipe", "YarnRecipeTypeId", c => c.Int());
            AddColumn("dbo.YarnRecipe", "ReportWireCount", c => c.Int());
            AddColumn("dbo.YarnRecipe", "MeterWireCount", c => c.Int());
            AddColumn("dbo.YarnRecipe", "Gramaj", c => c.Decimal(precision: 18, scale: 2));
            CreateIndex("dbo.ItemKnitDensity", "YarnRecipeTypeId");
            CreateIndex("dbo.YarnRecipe", "ItemId");
            CreateIndex("dbo.YarnRecipe", "YarnRecipeTypeId");
            AddForeignKey("dbo.YarnRecipe", "ItemId", "dbo.Item", "Id");
            AddForeignKey("dbo.YarnRecipe", "YarnRecipeTypeId", "dbo.YarnRecipeType", "Id");
            AddForeignKey("dbo.ItemKnitDensity", "YarnRecipeTypeId", "dbo.YarnRecipeType", "Id", cascadeDelete: true);
            DropColumn("dbo.Item", "Pattern");
            DropColumn("dbo.Item", "CrudeWidth");
            DropColumn("dbo.Item", "CrudeGramaj");
            DropColumn("dbo.Item", "ProductWidth");
            DropColumn("dbo.Item", "ProductGramaj");
            DropColumn("dbo.Item", "WarpWireCount");
            DropColumn("dbo.Item", "MeterGramaj");
            DropColumn("dbo.Item", "ItemCutType");
            DropColumn("dbo.Item", "ItemDyeHouseType");
            DropColumn("dbo.Item", "ItemApparelType");
            DropColumn("dbo.Item", "ItemBulletType");
            DropColumn("dbo.Item", "TestNo");
            DropColumn("dbo.Item", "CombWidth");
            DropColumn("dbo.Item", "WeftReportLength");
            DropColumn("dbo.Item", "WarpReportLength");
            DropColumn("dbo.Item", "WeftDensity");
            DropColumn("dbo.Item", "MachineId");
            DropColumn("dbo.ItemKnitDensity", "YarnRecipeType");
            DropColumn("dbo.YarnRecipe", "YarnBreedId");
            DropColumn("dbo.YarnRecipe", "Factor");
            DropColumn("dbo.YarnRecipe", "Twist");
            DropColumn("dbo.YarnRecipe", "CenterType");
            DropColumn("dbo.YarnRecipe", "Mix");
            DropColumn("dbo.YarnRecipe", "YarnRecipeType");
            DropColumn("dbo.YarnRecipe", "PlantId");
            DropColumn("dbo.YarnRecipe", "CreatedDate");
            DropColumn("dbo.YarnRecipe", "CreatedUserId");
            DropColumn("dbo.YarnRecipe", "UpdatedDate");
            DropColumn("dbo.YarnRecipe", "UpdatedUserId");
            DropColumn("dbo.YarnRecipe", "YarnColourId");
            DropColumn("dbo.YarnRecipe", "YarnLot");
            DropTable("dbo.FabricRecipe");
            DropTable("dbo.KnitYarn");
            DropTable("dbo.YarnBreed");
            DropTable("dbo.YarnRecipeMix");
            DropTable("dbo.YarnColour");
            DropTable("dbo.YarnColourGroup");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.YarnColourGroup",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        YarnColourGroupCode = c.String(),
                        YarnColourGroupName = c.String(),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.YarnColour",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        YarnColourCode = c.Int(nullable: false),
                        YarnColourName = c.String(),
                        YarnColourGroupId = c.Int(nullable: false),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.YarnRecipeMix",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        YarnRecipeId = c.Int(nullable: false),
                        YarnBreedId = c.Int(),
                        Percentage = c.Decimal(precision: 18, scale: 2),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.YarnBreed",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        YarnBreedCode = c.String(),
                        YarnBreedName = c.String(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.KnitYarn",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        YarnRecipeId = c.Int(nullable: false),
                        FirmId = c.Int(nullable: false),
                        ItemId = c.Int(nullable: false),
                        YarnType = c.Int(),
                        ReportWireCount = c.Decimal(precision: 18, scale: 2),
                        MeterWireCount = c.Decimal(precision: 18, scale: 2),
                        Gramaj = c.Decimal(precision: 18, scale: 2),
                        Density = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FabricRecipe",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemId = c.Int(),
                        FabricRecipeCode = c.String(),
                        FabricRecipeName = c.String(),
                        Denier = c.Int(),
                        FirmId = c.Int(),
                        YarnRecipeType = c.String(),
                        ReportWireCount = c.Int(),
                        MeterWireCount = c.Int(),
                        Gramaj = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.YarnRecipe", "YarnLot", c => c.Int());
            AddColumn("dbo.YarnRecipe", "YarnColourId", c => c.Int());
            AddColumn("dbo.YarnRecipe", "UpdatedUserId", c => c.Int());
            AddColumn("dbo.YarnRecipe", "UpdatedDate", c => c.DateTime());
            AddColumn("dbo.YarnRecipe", "CreatedUserId", c => c.Int());
            AddColumn("dbo.YarnRecipe", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.YarnRecipe", "PlantId", c => c.Int());
            AddColumn("dbo.YarnRecipe", "YarnRecipeType", c => c.Int());
            AddColumn("dbo.YarnRecipe", "Mix", c => c.Boolean());
            AddColumn("dbo.YarnRecipe", "CenterType", c => c.Int());
            AddColumn("dbo.YarnRecipe", "Twist", c => c.Int());
            AddColumn("dbo.YarnRecipe", "Factor", c => c.Int());
            AddColumn("dbo.YarnRecipe", "YarnBreedId", c => c.Int());
            AddColumn("dbo.ItemKnitDensity", "YarnRecipeType", c => c.String());
            AddColumn("dbo.Item", "MachineId", c => c.Int());
            AddColumn("dbo.Item", "WeftDensity", c => c.Int());
            AddColumn("dbo.Item", "WarpReportLength", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Item", "WeftReportLength", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Item", "CombWidth", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Item", "TestNo", c => c.Int());
            AddColumn("dbo.Item", "ItemBulletType", c => c.Int());
            AddColumn("dbo.Item", "ItemApparelType", c => c.Int());
            AddColumn("dbo.Item", "ItemDyeHouseType", c => c.Int());
            AddColumn("dbo.Item", "ItemCutType", c => c.Int());
            AddColumn("dbo.Item", "MeterGramaj", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Item", "WarpWireCount", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Item", "ProductGramaj", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Item", "ProductWidth", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Item", "CrudeGramaj", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Item", "CrudeWidth", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Item", "Pattern", c => c.Int());
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
            DropForeignKey("dbo.ItemKnitDensity", "YarnRecipeTypeId", "dbo.YarnRecipeType");
            DropForeignKey("dbo.YarnRecipe", "YarnRecipeTypeId", "dbo.YarnRecipeType");
            DropForeignKey("dbo.YarnRecipe", "ItemId", "dbo.Item");
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
            DropIndex("dbo.YarnRecipe", new[] { "YarnRecipeTypeId" });
            DropIndex("dbo.YarnRecipe", new[] { "ItemId" });
            DropIndex("dbo.ItemKnitDensity", new[] { "YarnRecipeTypeId" });
            DropColumn("dbo.YarnRecipe", "Gramaj");
            DropColumn("dbo.YarnRecipe", "MeterWireCount");
            DropColumn("dbo.YarnRecipe", "ReportWireCount");
            DropColumn("dbo.YarnRecipe", "YarnRecipeTypeId");
            DropColumn("dbo.YarnRecipe", "ItemId");
            DropColumn("dbo.ItemQualityType", "UpdatedUserId");
            DropColumn("dbo.ItemQualityType", "UpdatedDate");
            DropColumn("dbo.ItemQualityType", "CreatedUserId");
            DropColumn("dbo.ItemQualityType", "CreatedDate");
            DropColumn("dbo.ItemQualityType", "IsActive");
            DropColumn("dbo.ItemKnitDensity", "YarnRecipeTypeId");
            DropTable("dbo.VehicleNotification");
            DropTable("dbo.VehicleType");
            DropTable("dbo.VehicleTire");
            DropTable("dbo.VehicleInsuranceType");
            DropTable("dbo.VehicleInsurance");
            DropTable("dbo.VehicleCareType");
            DropTable("dbo.VehicleCare");
            DropTable("dbo.Vehicle");
            DropTable("dbo.YarnRecipeType");
            CreateIndex("dbo.YarnColour", "YarnColourGroupId");
            CreateIndex("dbo.YarnRecipeMix", "YarnBreedId");
            CreateIndex("dbo.YarnRecipeMix", "YarnRecipeId");
            CreateIndex("dbo.YarnRecipe", "YarnColourId");
            CreateIndex("dbo.YarnRecipe", "YarnBreedId");
            CreateIndex("dbo.KnitYarn", "ItemId");
            CreateIndex("dbo.KnitYarn", "FirmId");
            CreateIndex("dbo.KnitYarn", "YarnRecipeId");
            CreateIndex("dbo.FabricRecipe", "FirmId");
            CreateIndex("dbo.FabricRecipe", "ItemId");
            CreateIndex("dbo.Item", "MachineId");
            AddForeignKey("dbo.Item", "MachineId", "dbo.Machine", "Id");
            AddForeignKey("dbo.KnitYarn", "ItemId", "dbo.Item", "Id", cascadeDelete: true);
            AddForeignKey("dbo.YarnRecipeMix", "YarnRecipeId", "dbo.YarnRecipe", "Id", cascadeDelete: true);
            AddForeignKey("dbo.YarnRecipe", "YarnColourId", "dbo.YarnColour", "Id");
            AddForeignKey("dbo.YarnColour", "YarnColourGroupId", "dbo.YarnColourGroup", "Id", cascadeDelete: true);
            AddForeignKey("dbo.YarnRecipeMix", "YarnBreedId", "dbo.YarnBreed", "Id");
            AddForeignKey("dbo.YarnRecipe", "YarnBreedId", "dbo.YarnBreed", "Id");
            AddForeignKey("dbo.KnitYarn", "YarnRecipeId", "dbo.YarnRecipe", "Id", cascadeDelete: true);
            AddForeignKey("dbo.KnitYarn", "FirmId", "dbo.Firm", "Id", cascadeDelete: true);
            AddForeignKey("dbo.FabricRecipe", "ItemId", "dbo.Item", "Id");
            AddForeignKey("dbo.FabricRecipe", "FirmId", "dbo.Firm", "Id");
        }
    }
}
