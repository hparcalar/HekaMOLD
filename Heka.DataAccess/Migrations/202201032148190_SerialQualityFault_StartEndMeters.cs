namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SerialQualityFault_StartEndMeters : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SerialQualityWindingFault", "EndMeter", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.SerialQualityWindingFault", "EndQuantity", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SerialQualityWindingFault", "EndQuantity");
            DropColumn("dbo.SerialQualityWindingFault", "EndMeter");
        }
    }
}
