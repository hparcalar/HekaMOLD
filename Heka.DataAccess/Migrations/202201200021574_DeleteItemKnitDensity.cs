namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteItemKnitDensity : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.YarnRecipe", "FirmId", "dbo.Firm");
            DropForeignKey("dbo.YarnRecipe", "ItemId", "dbo.Item");
            DropForeignKey("dbo.YarnRecipe", "YarnRecipeTypeId", "dbo.YarnRecipeType");
            DropForeignKey("dbo.ItemKnitDensity", "YarnRecipeTypeId", "dbo.YarnRecipeType");
            DropForeignKey("dbo.ItemKnitDensity", "ItemId", "dbo.Item");
            DropIndex("dbo.ItemKnitDensity", new[] { "ItemId" });
            DropIndex("dbo.ItemKnitDensity", new[] { "YarnRecipeTypeId" });
            DropIndex("dbo.YarnRecipe", new[] { "ItemId" });
            DropIndex("dbo.YarnRecipe", new[] { "FirmId" });
            DropIndex("dbo.YarnRecipe", new[] { "YarnRecipeTypeId" });
            DropTable("dbo.ItemKnitDensity");
            DropTable("dbo.YarnRecipeType");
            DropTable("dbo.YarnRecipe");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.YarnRecipe",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemId = c.Int(),
                        YarnRecipeCode = c.String(),
                        YarnRecipeName = c.String(),
                        FirmId = c.Int(),
                        YarnRecipeTypeId = c.Int(),
                        Denier = c.Int(),
                        ReportWireCount = c.Int(),
                        MeterWireCount = c.Int(),
                        Gramaj = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
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
                "dbo.ItemKnitDensity",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemId = c.Int(nullable: false),
                        YarnRecipeTypeId = c.Int(nullable: false),
                        Density = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.YarnRecipe", "YarnRecipeTypeId");
            CreateIndex("dbo.YarnRecipe", "FirmId");
            CreateIndex("dbo.YarnRecipe", "ItemId");
            CreateIndex("dbo.ItemKnitDensity", "YarnRecipeTypeId");
            CreateIndex("dbo.ItemKnitDensity", "ItemId");
            AddForeignKey("dbo.ItemKnitDensity", "ItemId", "dbo.Item", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ItemKnitDensity", "YarnRecipeTypeId", "dbo.YarnRecipeType", "Id", cascadeDelete: true);
            AddForeignKey("dbo.YarnRecipe", "YarnRecipeTypeId", "dbo.YarnRecipeType", "Id");
            AddForeignKey("dbo.YarnRecipe", "ItemId", "dbo.Item", "Id");
            AddForeignKey("dbo.YarnRecipe", "FirmId", "dbo.Firm", "Id");
        }
    }
}
