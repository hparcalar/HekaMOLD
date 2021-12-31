namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class intConvertDecimalWeftReportLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Item", "WeftReportLength", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Item", "WeftReportLength", c => c.Int());
        }
    }
}
