namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class YarnRecipeMixModeladded : DbMigration
    {
        public override void Up()
        {
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.YarnBreed", t => t.YarnBreedId)
                .ForeignKey("dbo.YarnRecipe", t => t.YarnRecipeId, cascadeDelete: true)
                .Index(t => t.YarnRecipeId)
                .Index(t => t.YarnBreedId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.YarnRecipeMix", "YarnRecipeId", "dbo.YarnRecipe");
            DropForeignKey("dbo.YarnRecipeMix", "YarnBreedId", "dbo.YarnBreed");
            DropIndex("dbo.YarnRecipeMix", new[] { "YarnBreedId" });
            DropIndex("dbo.YarnRecipeMix", new[] { "YarnRecipeId" });
            DropTable("dbo.YarnRecipeMix");
        }
    }
}
