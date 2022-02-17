namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class YarnRecipeUpdatePriceAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.YarnRecipe", "Price", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.YarnRecipe", "ForexTypeId", c => c.Int());
            CreateIndex("dbo.YarnRecipe", "ForexTypeId");
            AddForeignKey("dbo.YarnRecipe", "ForexTypeId", "dbo.ForexType", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.YarnRecipe", "ForexTypeId", "dbo.ForexType");
            DropIndex("dbo.YarnRecipe", new[] { "ForexTypeId" });
            DropColumn("dbo.YarnRecipe", "ForexTypeId");
            DropColumn("dbo.YarnRecipe", "Price");
        }
    }
}
