namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageDetailUpdateDischargerPointTypeAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VoyageDetail", "DischangePointType", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VoyageDetail", "DischangePointType");
        }
    }
}
