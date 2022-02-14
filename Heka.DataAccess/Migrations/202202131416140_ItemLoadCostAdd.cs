namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadCostAdd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemLoadCost",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemLoadId = c.Int(nullable: false),
                        LoadCostCategoryId = c.Int(nullable: false),
                        CostPrice = c.Decimal(precision: 18, scale: 2),
                        ForexTypeId = c.Int(),
                        Explanation = c.String(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LoadCostCategory", t => t.LoadCostCategoryId, cascadeDelete: true)
                .ForeignKey("dbo.ItemLoad", t => t.ItemLoadId, cascadeDelete: true)
                .Index(t => t.ItemLoadId)
                .Index(t => t.LoadCostCategoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemLoadCost", "ItemLoadId", "dbo.ItemLoad");
            DropForeignKey("dbo.ItemLoadCost", "LoadCostCategoryId", "dbo.LoadCostCategory");
            DropIndex("dbo.ItemLoadCost", new[] { "LoadCostCategoryId" });
            DropIndex("dbo.ItemLoadCost", new[] { "ItemLoadId" });
            DropTable("dbo.ItemLoadCost");
        }
    }
}
