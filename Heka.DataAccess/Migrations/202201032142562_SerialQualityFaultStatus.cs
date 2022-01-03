namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SerialQualityFaultStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SerialQualityWindingFault", "FaultStatus", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SerialQualityWindingFault", "FaultStatus");
        }
    }
}
