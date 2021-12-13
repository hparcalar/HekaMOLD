namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnUser : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.YarnRecipe", "StockCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.YarnRecipe", "StockCode", c => c.String());
        }
    }
}
