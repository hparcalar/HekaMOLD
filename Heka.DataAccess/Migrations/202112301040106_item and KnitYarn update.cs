namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class itemandKnitYarnupdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Item", "CrudeWidth", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Item", "ProductWidth", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Item", "WarpWireCount", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Item", "CombWidth", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Item", "WarpReportLength", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.KnitYarn", "ReportWireCount", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.KnitYarn", "MeterWireCount", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.KnitYarn", "MeterWireCount", c => c.Int());
            AlterColumn("dbo.KnitYarn", "ReportWireCount", c => c.Int());
            AlterColumn("dbo.Item", "WarpReportLength", c => c.Int());
            AlterColumn("dbo.Item", "CombWidth", c => c.Int());
            AlterColumn("dbo.Item", "WarpWireCount", c => c.Int());
            AlterColumn("dbo.Item", "ProductWidth", c => c.Int());
            AlterColumn("dbo.Item", "CrudeWidth", c => c.Int());
        }
    }
}
