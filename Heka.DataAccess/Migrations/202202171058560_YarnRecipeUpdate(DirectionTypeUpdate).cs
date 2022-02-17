namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class YarnRecipeUpdateDirectionTypeUpdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.YarnRecipe", "TwistDirection", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.YarnRecipe", "TwistDirection", c => c.String());
        }
    }
}
