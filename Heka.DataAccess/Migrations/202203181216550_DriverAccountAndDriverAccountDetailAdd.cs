namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DriverAccountAndDriverAccountDetailAdd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DriverAccount",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DriverId = c.Int(),
                        ForexTypeId = c.Int(),
                        Balance = c.Decimal(precision: 18, scale: 2),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ForexType", t => t.ForexTypeId)
                .ForeignKey("dbo.Driver", t => t.DriverId)
                .Index(t => t.DriverId)
                .Index(t => t.ForexTypeId);
            
            CreateTable(
                "dbo.DriverAccountDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DriverId = c.Int(nullable: false),
                        DriverAccountId = c.Int(nullable: false),
                        ForexTypeId = c.Int(nullable: false),
                        OverallTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ActionType = c.Int(nullable: false),
                        VoyageCostDetailId = c.Int(),
                        VoyageId = c.Int(),
                        CostCategoryId = c.Int(),
                        DocumentNo = c.String(),
                        Explanation = c.String(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CostCategory", t => t.CostCategoryId)
                .ForeignKey("dbo.VoyageCostDetail", t => t.VoyageCostDetailId)
                .ForeignKey("dbo.ForexType", t => t.ForexTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Voyage", t => t.VoyageId)
                .ForeignKey("dbo.DriverAccount", t => t.DriverAccountId, cascadeDelete: true)
                .ForeignKey("dbo.Driver", t => t.DriverId, cascadeDelete: true)
                .Index(t => t.DriverId)
                .Index(t => t.DriverAccountId)
                .Index(t => t.ForexTypeId)
                .Index(t => t.VoyageCostDetailId)
                .Index(t => t.VoyageId)
                .Index(t => t.CostCategoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DriverAccountDetail", "DriverId", "dbo.Driver");
            DropForeignKey("dbo.DriverAccount", "DriverId", "dbo.Driver");
            DropForeignKey("dbo.DriverAccountDetail", "DriverAccountId", "dbo.DriverAccount");
            DropForeignKey("dbo.DriverAccountDetail", "VoyageId", "dbo.Voyage");
            DropForeignKey("dbo.DriverAccountDetail", "ForexTypeId", "dbo.ForexType");
            DropForeignKey("dbo.DriverAccount", "ForexTypeId", "dbo.ForexType");
            DropForeignKey("dbo.DriverAccountDetail", "VoyageCostDetailId", "dbo.VoyageCostDetail");
            DropForeignKey("dbo.DriverAccountDetail", "CostCategoryId", "dbo.CostCategory");
            DropIndex("dbo.DriverAccountDetail", new[] { "CostCategoryId" });
            DropIndex("dbo.DriverAccountDetail", new[] { "VoyageId" });
            DropIndex("dbo.DriverAccountDetail", new[] { "VoyageCostDetailId" });
            DropIndex("dbo.DriverAccountDetail", new[] { "ForexTypeId" });
            DropIndex("dbo.DriverAccountDetail", new[] { "DriverAccountId" });
            DropIndex("dbo.DriverAccountDetail", new[] { "DriverId" });
            DropIndex("dbo.DriverAccount", new[] { "ForexTypeId" });
            DropIndex("dbo.DriverAccount", new[] { "DriverId" });
            DropTable("dbo.DriverAccountDetail");
            DropTable("dbo.DriverAccount");
        }
    }
}
