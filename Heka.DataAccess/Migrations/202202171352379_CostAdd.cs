namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CostAdd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cost",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CostCode = c.String(),
                        CostName = c.String(),
                        Quantity = c.Int(),
                        UnitPrice = c.Decimal(precision: 18, scale: 2),
                        OverallTotal = c.Decimal(precision: 18, scale: 2),
                        Explanation = c.String(),
                        ForexTypeId = c.Int(),
                        UnitTypeId = c.Int(),
                        CostCategoryId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CostCategory", t => t.CostCategoryId)
                .ForeignKey("dbo.UnitType", t => t.UnitTypeId)
                .ForeignKey("dbo.ForexType", t => t.ForexTypeId)
                .Index(t => t.ForexTypeId)
                .Index(t => t.UnitTypeId)
                .Index(t => t.CostCategoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Cost", "ForexTypeId", "dbo.ForexType");
            DropForeignKey("dbo.Cost", "UnitTypeId", "dbo.UnitType");
            DropForeignKey("dbo.Cost", "CostCategoryId", "dbo.CostCategory");
            DropIndex("dbo.Cost", new[] { "CostCategoryId" });
            DropIndex("dbo.Cost", new[] { "UnitTypeId" });
            DropIndex("dbo.Cost", new[] { "ForexTypeId" });
            DropTable("dbo.Cost");
        }
    }
}
