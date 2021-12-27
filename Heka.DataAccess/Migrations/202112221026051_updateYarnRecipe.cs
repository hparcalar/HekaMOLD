namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateYarnRecipe : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.YarnRecipe", "YarnRecipeType", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.YarnRecipe", "YarnRecipeType");
        }
    }
}
