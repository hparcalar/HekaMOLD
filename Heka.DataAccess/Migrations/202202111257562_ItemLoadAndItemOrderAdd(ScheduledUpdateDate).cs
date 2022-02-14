namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadAndItemOrderAddScheduledUpdateDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "ScheduledUploadDate", c => c.DateTime());
            AddColumn("dbo.ItemOrder", "ScheduledUploadDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemOrder", "ScheduledUploadDate");
            DropColumn("dbo.ItemLoad", "ScheduledUploadDate");
        }
    }
}
