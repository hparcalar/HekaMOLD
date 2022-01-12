namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemUpdateAvedateWeftDensity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Item", "AverageWeftDensity", c => c.Int());
            AddColumn("dbo.Item", "AverageWarpDensity", c => c.Int());
            DropColumn("dbo.Item", "WeftDensity");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Item", "WeftDensity", c => c.Int());
            DropColumn("dbo.Item", "AverageWarpDensity");
            DropColumn("dbo.Item", "AverageWeftDensity");
        }
    }
}
