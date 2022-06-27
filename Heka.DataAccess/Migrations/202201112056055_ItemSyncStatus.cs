namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemSyncStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Item", "SyncStatus", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Item", "SyncStatus");
        }
    }
}
