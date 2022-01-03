namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SerialQualityWinding_TotalMetersAndQuantity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SerialQualityWinding", "TotalMeters", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.SerialQualityWinding", "TotalQuantity", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SerialQualityWinding", "TotalQuantity");
            DropColumn("dbo.SerialQualityWinding", "TotalMeters");
        }
    }
}
