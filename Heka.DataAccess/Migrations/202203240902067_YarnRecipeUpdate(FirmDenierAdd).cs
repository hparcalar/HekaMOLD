namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class YarnRecipeUpdateFirmDenierAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.YarnRecipe", "YarnDenier", c => c.Int());
            AlterColumn("dbo.YarnRecipe", "YarnLot", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.YarnRecipe", "YarnLot", c => c.Int());
            DropColumn("dbo.YarnRecipe", "YarnDenier");
        }
    }
}
