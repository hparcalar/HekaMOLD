namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateT1T2StartDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "T1T2StartDate", c => c.DateTime());
            AddColumn("dbo.VoyageDetail", "T1T2StartDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VoyageDetail", "T1T2StartDate");
            DropColumn("dbo.ItemLoad", "T1T2StartDate");
        }
    }
}
