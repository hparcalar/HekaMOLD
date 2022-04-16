namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageAndItemLoadUpdateWeekUpdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ItemOrder", "OrderWeek", c => c.String());
            AlterColumn("dbo.ItemLoad", "LoadWeek", c => c.String());
            AlterColumn("dbo.Voyage", "VoyageWeek", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Voyage", "VoyageWeek", c => c.Int());
            AlterColumn("dbo.ItemLoad", "LoadWeek", c => c.Int());
            AlterColumn("dbo.ItemOrder", "OrderWeek", c => c.Int());
        }
    }
}
