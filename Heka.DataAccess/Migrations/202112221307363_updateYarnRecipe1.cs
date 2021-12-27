namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateYarnRecipe1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.YarnRecipe", "YarnBreedId", "dbo.YarnBreed");
            DropForeignKey("dbo.YarnRecipe", "YarnColourId", "dbo.YarnColour");
            DropIndex("dbo.YarnRecipe", new[] { "YarnBreedId" });
            DropIndex("dbo.YarnRecipe", new[] { "YarnColourId" });
            AlterColumn("dbo.YarnRecipe", "YarnBreedId", c => c.Int());
            AlterColumn("dbo.YarnRecipe", "YarnColourId", c => c.Int());
            AlterColumn("dbo.YarnRecipe", "YarnLot", c => c.Int());
            CreateIndex("dbo.YarnRecipe", "YarnBreedId");
            CreateIndex("dbo.YarnRecipe", "YarnColourId");
            AddForeignKey("dbo.YarnRecipe", "YarnBreedId", "dbo.YarnBreed", "Id");
            AddForeignKey("dbo.YarnRecipe", "YarnColourId", "dbo.YarnColour", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.YarnRecipe", "YarnColourId", "dbo.YarnColour");
            DropForeignKey("dbo.YarnRecipe", "YarnBreedId", "dbo.YarnBreed");
            DropIndex("dbo.YarnRecipe", new[] { "YarnColourId" });
            DropIndex("dbo.YarnRecipe", new[] { "YarnBreedId" });
            AlterColumn("dbo.YarnRecipe", "YarnLot", c => c.Int(nullable: false));
            AlterColumn("dbo.YarnRecipe", "YarnColourId", c => c.Int(nullable: false));
            AlterColumn("dbo.YarnRecipe", "YarnBreedId", c => c.Int(nullable: false));
            CreateIndex("dbo.YarnRecipe", "YarnColourId");
            CreateIndex("dbo.YarnRecipe", "YarnBreedId");
            AddForeignKey("dbo.YarnRecipe", "YarnColourId", "dbo.YarnColour", "Id", cascadeDelete: true);
            AddForeignKey("dbo.YarnRecipe", "YarnBreedId", "dbo.YarnBreed", "Id", cascadeDelete: true);
        }
    }
}
