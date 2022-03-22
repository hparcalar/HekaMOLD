namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageCostUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VoyageCostDetail", "PayType", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VoyageCostDetail", "PayType");
        }
    }
}
