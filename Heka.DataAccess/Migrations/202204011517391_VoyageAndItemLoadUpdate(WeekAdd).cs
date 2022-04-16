namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageAndItemLoadUpdateWeekAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "LoadWeek", c => c.Int());
            AddColumn("dbo.Voyage", "VoyageWeek", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Voyage", "VoyageWeek");
            DropColumn("dbo.ItemLoad", "LoadWeek");
        }
    }
}
