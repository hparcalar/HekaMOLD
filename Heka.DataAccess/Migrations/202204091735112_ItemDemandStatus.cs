namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemDemandStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemDemand", "DemandStatus", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemDemand", "DemandStatus");
        }
    }
}
