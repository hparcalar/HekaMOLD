namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateLoadStatusdUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "LoadStatusType", c => c.Int());
            DropColumn("dbo.ItemLoad", "OrderLoadStatusType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemLoad", "OrderLoadStatusType", c => c.Int());
            DropColumn("dbo.ItemLoad", "LoadStatusType");
        }
    }
}
