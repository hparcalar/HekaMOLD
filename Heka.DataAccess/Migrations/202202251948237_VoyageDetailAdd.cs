namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageDetailAdd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VoyageDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LoadCode = c.String(),
                        OrderNo = c.String(),
                        LoadDate = c.DateTime(),
                        DischargeDate = c.DateTime(),
                        VoyageStatus = c.Int(),
                        OveralWeight = c.Decimal(precision: 18, scale: 2),
                        OveralVolume = c.Decimal(precision: 18, scale: 2),
                        OveralLadametre = c.Decimal(precision: 18, scale: 2),
                        OverallTotal = c.Decimal(precision: 18, scale: 2),
                        CalculationTypePrice = c.Decimal(precision: 18, scale: 2),
                        DocumentNo = c.String(),
                        OrderUploadType = c.Int(),
                        OrderUploadPointType = c.Int(),
                        OrderTransactionDirectionType = c.Int(),
                        OrderCalculationType = c.Int(),
                        LoadOutDate = c.DateTime(),
                        ScheduledUploadDate = c.DateTime(),
                        DateOfNeed = c.DateTime(),
                        OveralQuantity = c.Int(),
                        LoadingDate = c.DateTime(),
                        InvoiceId = c.Int(),
                        ForexTypeId = c.Int(),
                        VehicleTraillerId = c.Int(),
                        InvoiceStatus = c.Int(),
                        InvoiceFreightPrice = c.Decimal(precision: 18, scale: 2),
                        CmrNo = c.String(),
                        CmrStatus = c.Int(),
                        Explanation = c.String(),
                        ShipperFirmExplanation = c.String(),
                        BuyerFirmExplanation = c.String(),
                        ReadinessDate = c.DateTime(),
                        DeliveryFromCustomerDate = c.DateTime(),
                        IntendedArrivalDate = c.DateTime(),
                        FirmCustomsArrivalId = c.Int(),
                        CustomsExplanation = c.String(),
                        T1T2No = c.String(),
                        TClosingDate = c.DateTime(),
                        HasCmrDeliveryed = c.Boolean(),
                        ItemPrice = c.Decimal(precision: 18, scale: 2),
                        TrailerType = c.Int(),
                        HasItemInsurance = c.Boolean(),
                        ItemInsuranceDraftNo = c.String(),
                        HasItemDangerous = c.Boolean(),
                        CmrCustomerDeliveryDate = c.DateTime(),
                        BringingToWarehousePlate = c.String(),
                        ShipperCityId = c.Int(),
                        BuyerCityId = c.Int(),
                        ShipperCountryId = c.Int(),
                        BuyerCountryId = c.Int(),
                        CustomerFirmId = c.Int(),
                        ShipperFirmId = c.Int(),
                        BuyerFirmId = c.Int(),
                        EntryCustomsId = c.Int(),
                        ExitCustomsId = c.Int(),
                        ItemLoadId = c.Int(),
                        VoyageId = c.Int(),
                        PlantId = c.Int(),
                        CreatedUserId = c.Int(),
                        CreatDate = c.DateTime(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.CreatedUserId)
                .ForeignKey("dbo.Vehicle", t => t.VehicleTraillerId)
                .ForeignKey("dbo.Invoice", t => t.InvoiceId)
                .ForeignKey("dbo.ForexType", t => t.ForexTypeId)
                .ForeignKey("dbo.ItemLoad", t => t.ItemLoadId)
                .ForeignKey("dbo.Customs", t => t.EntryCustomsId)
                .ForeignKey("dbo.Customs", t => t.ExitCustomsId)
                .ForeignKey("dbo.Voyage", t => t.VoyageId)
                .ForeignKey("dbo.Country", t => t.BuyerCountryId)
                .ForeignKey("dbo.Country", t => t.ShipperCountryId)
                .ForeignKey("dbo.City", t => t.BuyerCityId)
                .ForeignKey("dbo.City", t => t.ShipperCityId)
                .ForeignKey("dbo.Firm", t => t.BuyerFirmId)
                .ForeignKey("dbo.Firm", t => t.CustomerFirmId)
                .ForeignKey("dbo.Firm", t => t.FirmCustomsArrivalId)
                .ForeignKey("dbo.Firm", t => t.ShipperFirmId)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.InvoiceId)
                .Index(t => t.ForexTypeId)
                .Index(t => t.VehicleTraillerId)
                .Index(t => t.FirmCustomsArrivalId)
                .Index(t => t.ShipperCityId)
                .Index(t => t.BuyerCityId)
                .Index(t => t.ShipperCountryId)
                .Index(t => t.BuyerCountryId)
                .Index(t => t.CustomerFirmId)
                .Index(t => t.ShipperFirmId)
                .Index(t => t.BuyerFirmId)
                .Index(t => t.EntryCustomsId)
                .Index(t => t.ExitCustomsId)
                .Index(t => t.ItemLoadId)
                .Index(t => t.VoyageId)
                .Index(t => t.PlantId)
                .Index(t => t.CreatedUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VoyageDetail", "PlantId", "dbo.Plant");
            DropForeignKey("dbo.VoyageDetail", "ShipperFirmId", "dbo.Firm");
            DropForeignKey("dbo.VoyageDetail", "FirmCustomsArrivalId", "dbo.Firm");
            DropForeignKey("dbo.VoyageDetail", "CustomerFirmId", "dbo.Firm");
            DropForeignKey("dbo.VoyageDetail", "BuyerFirmId", "dbo.Firm");
            DropForeignKey("dbo.VoyageDetail", "ShipperCityId", "dbo.City");
            DropForeignKey("dbo.VoyageDetail", "BuyerCityId", "dbo.City");
            DropForeignKey("dbo.VoyageDetail", "ShipperCountryId", "dbo.Country");
            DropForeignKey("dbo.VoyageDetail", "BuyerCountryId", "dbo.Country");
            DropForeignKey("dbo.VoyageDetail", "VoyageId", "dbo.Voyage");
            DropForeignKey("dbo.VoyageDetail", "ExitCustomsId", "dbo.Customs");
            DropForeignKey("dbo.VoyageDetail", "EntryCustomsId", "dbo.Customs");
            DropForeignKey("dbo.VoyageDetail", "ItemLoadId", "dbo.ItemLoad");
            DropForeignKey("dbo.VoyageDetail", "ForexTypeId", "dbo.ForexType");
            DropForeignKey("dbo.VoyageDetail", "InvoiceId", "dbo.Invoice");
            DropForeignKey("dbo.VoyageDetail", "VehicleTraillerId", "dbo.Vehicle");
            DropForeignKey("dbo.VoyageDetail", "CreatedUserId", "dbo.User");
            DropIndex("dbo.VoyageDetail", new[] { "CreatedUserId" });
            DropIndex("dbo.VoyageDetail", new[] { "PlantId" });
            DropIndex("dbo.VoyageDetail", new[] { "VoyageId" });
            DropIndex("dbo.VoyageDetail", new[] { "ItemLoadId" });
            DropIndex("dbo.VoyageDetail", new[] { "ExitCustomsId" });
            DropIndex("dbo.VoyageDetail", new[] { "EntryCustomsId" });
            DropIndex("dbo.VoyageDetail", new[] { "BuyerFirmId" });
            DropIndex("dbo.VoyageDetail", new[] { "ShipperFirmId" });
            DropIndex("dbo.VoyageDetail", new[] { "CustomerFirmId" });
            DropIndex("dbo.VoyageDetail", new[] { "BuyerCountryId" });
            DropIndex("dbo.VoyageDetail", new[] { "ShipperCountryId" });
            DropIndex("dbo.VoyageDetail", new[] { "BuyerCityId" });
            DropIndex("dbo.VoyageDetail", new[] { "ShipperCityId" });
            DropIndex("dbo.VoyageDetail", new[] { "FirmCustomsArrivalId" });
            DropIndex("dbo.VoyageDetail", new[] { "VehicleTraillerId" });
            DropIndex("dbo.VoyageDetail", new[] { "ForexTypeId" });
            DropIndex("dbo.VoyageDetail", new[] { "InvoiceId" });
            DropTable("dbo.VoyageDetail");
        }
    }
}
