namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateItemModels : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Item", "CombWidth", c => c.Int());
            AlterColumn("dbo.Item", "WeftReportLength", c => c.Int());
            AlterColumn("dbo.Item", "WarpReportLength", c => c.Int());
            AlterColumn("dbo.Item", "WeftDensity", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Item", "WeftDensity", c => c.Int(nullable: false));
            AlterColumn("dbo.Item", "WarpReportLength", c => c.Int(nullable: false));
            AlterColumn("dbo.Item", "WeftReportLength", c => c.Int(nullable: false));
            AlterColumn("dbo.Item", "CombWidth", c => c.Int(nullable: false));
        }
    }
}
