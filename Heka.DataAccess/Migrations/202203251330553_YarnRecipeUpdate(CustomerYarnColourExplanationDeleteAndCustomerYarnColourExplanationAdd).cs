namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class YarnRecipeUpdateCustomerYarnColourExplanationDeleteAndCustomerYarnColourExplanationAdd : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.YarnRecipe", name: "CustomerYarnColourId", newName: "CustomerYarnColour_Id");
            RenameIndex(table: "dbo.YarnRecipe", name: "IX_CustomerYarnColourId", newName: "IX_CustomerYarnColour_Id");
            AddColumn("dbo.YarnRecipe", "CustomerYarnColorExplanation", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.YarnRecipe", "CustomerYarnColorExplanation");
            RenameIndex(table: "dbo.YarnRecipe", name: "IX_CustomerYarnColour_Id", newName: "IX_CustomerYarnColourId");
            RenameColumn(table: "dbo.YarnRecipe", name: "CustomerYarnColour_Id", newName: "CustomerYarnColourId");
        }
    }
}
