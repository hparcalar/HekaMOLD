namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateYarnRecipe2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.YarnRecipe", "CenterType", c => c.Int());
            DropColumn("dbo.YarnRecipe", "Center");
        }
        
        public override void Down()
        {
            AddColumn("dbo.YarnRecipe", "Center", c => c.Boolean());
            DropColumn("dbo.YarnRecipe", "CenterType");
        }
    }
}
