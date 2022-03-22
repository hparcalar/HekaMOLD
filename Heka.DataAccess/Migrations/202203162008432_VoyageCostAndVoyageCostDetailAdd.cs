namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageCostAndVoyageCostDetailAdd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VoyageCost",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VoyageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Voyage", t => t.VoyageId, cascadeDelete: true)
                .Index(t => t.VoyageId);
            
            CreateTable(
                "dbo.VoyageCostDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VoyageCostId = c.Int(),
                        DriverId = c.Int(),
                        CountryId = c.Int(),
                        CostCategoryId = c.Int(),
                        OperationDate = c.DateTime(),
                        Quantity = c.Int(),
                        UnitTypeId = c.Int(),
                        OverallTotal = c.Decimal(precision: 18, scale: 2),
                        ForexTypeId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CostCategory", t => t.CostCategoryId)
                .ForeignKey("dbo.VoyageCost", t => t.VoyageCostId)
                .ForeignKey("dbo.UnitType", t => t.UnitTypeId)
                .ForeignKey("dbo.ForexType", t => t.ForexTypeId)
                .ForeignKey("dbo.Driver", t => t.DriverId)
                .ForeignKey("dbo.Country", t => t.CountryId)
                .Index(t => t.VoyageCostId)
                .Index(t => t.DriverId)
                .Index(t => t.CountryId)
                .Index(t => t.CostCategoryId)
                .Index(t => t.UnitTypeId)
                .Index(t => t.ForexTypeId);
            
            CreateTable(
                "dbo.CostCategory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CostCategoryCode = c.String(),
                        CostCategoryName = c.String(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VoyageCostDetail", "CountryId", "dbo.Country");
            DropForeignKey("dbo.VoyageCostDetail", "DriverId", "dbo.Driver");
            DropForeignKey("dbo.VoyageCostDetail", "ForexTypeId", "dbo.ForexType");
            DropForeignKey("dbo.VoyageCostDetail", "UnitTypeId", "dbo.UnitType");
            DropForeignKey("dbo.VoyageCost", "VoyageId", "dbo.Voyage");
            DropForeignKey("dbo.VoyageCostDetail", "VoyageCostId", "dbo.VoyageCost");
            DropForeignKey("dbo.VoyageCostDetail", "CostCategoryId", "dbo.CostCategory");
            DropIndex("dbo.VoyageCostDetail", new[] { "ForexTypeId" });
            DropIndex("dbo.VoyageCostDetail", new[] { "UnitTypeId" });
            DropIndex("dbo.VoyageCostDetail", new[] { "CostCategoryId" });
            DropIndex("dbo.VoyageCostDetail", new[] { "CountryId" });
            DropIndex("dbo.VoyageCostDetail", new[] { "DriverId" });
            DropIndex("dbo.VoyageCostDetail", new[] { "VoyageCostId" });
            DropIndex("dbo.VoyageCost", new[] { "VoyageId" });
            DropTable("dbo.CostCategory");
            DropTable("dbo.VoyageCostDetail");
            DropTable("dbo.VoyageCost");
        }
    }
}
