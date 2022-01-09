namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class YarnRecipeAddedTwistDirection : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.YarnRecipe", "TwistDirection", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.YarnRecipe", "TwistDirection");
        }
    }
}
