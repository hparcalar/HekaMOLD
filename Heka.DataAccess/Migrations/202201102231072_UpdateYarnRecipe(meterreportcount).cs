namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateYarnRecipemeterreportcount : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.KnitYarn", "MeterWireCount", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.KnitYarn", "MeterWireCount", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}
