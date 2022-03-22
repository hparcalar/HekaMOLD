namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateOrderDateAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "OrderDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemLoad", "OrderDate");
        }
    }
}
