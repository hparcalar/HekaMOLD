namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateWarehouseDateAdd2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "BringingToWarehouseDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemLoad", "BringingToWarehouseDate");
        }
    }
}
