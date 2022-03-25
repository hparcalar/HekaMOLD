namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class YarnRecipeUpdateExplanationAndCustomerYarnColourIdAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.YarnRecipe", "Explanation", c => c.String());
            AddColumn("dbo.YarnRecipe", "CustomerYarnColourId", c => c.Int());
            CreateIndex("dbo.YarnRecipe", "CustomerYarnColourId");
            AddForeignKey("dbo.YarnRecipe", "CustomerYarnColourId", "dbo.YarnColour", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.YarnRecipe", "CustomerYarnColourId", "dbo.YarnColour");
            DropIndex("dbo.YarnRecipe", new[] { "CustomerYarnColourId" });
            DropColumn("dbo.YarnRecipe", "CustomerYarnColourId");
            DropColumn("dbo.YarnRecipe", "Explanation");
        }
    }
}
