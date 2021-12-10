namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemKnitDensity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemKnitDensity",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemId = c.Int(nullable: false),
                        YarnRecipeTypeId = c.Int(nullable: false),
                        Density = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Item", t => t.ItemId, cascadeDelete: true)
                .ForeignKey("dbo.YarnRecipeType", t => t.YarnRecipeTypeId, cascadeDelete: true)
                .Index(t => t.ItemId)
                .Index(t => t.YarnRecipeTypeId);
            
            CreateTable(
                "dbo.YarnRecipeType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        YarnTypeCode = c.String(),
                        YarnTypeName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.YarnRecipe", "YarnRecipeTypeId");
            AddForeignKey("dbo.YarnRecipe", "YarnRecipeTypeId", "dbo.YarnRecipeType", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemKnitDensity", "YarnRecipeTypeId", "dbo.YarnRecipeType");
            DropForeignKey("dbo.YarnRecipe", "YarnRecipeTypeId", "dbo.YarnRecipeType");
            DropForeignKey("dbo.ItemKnitDensity", "ItemId", "dbo.Item");
            DropIndex("dbo.YarnRecipe", new[] { "YarnRecipeTypeId" });
            DropIndex("dbo.ItemKnitDensity", new[] { "YarnRecipeTypeId" });
            DropIndex("dbo.ItemKnitDensity", new[] { "ItemId" });
            DropTable("dbo.YarnRecipeType");
            DropTable("dbo.ItemKnitDensity");
        }
    }
}
