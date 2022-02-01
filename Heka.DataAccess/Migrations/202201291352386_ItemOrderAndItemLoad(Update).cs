namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemOrderAndItemLoadUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "OveralQuantity", c => c.Int());
            AddColumn("dbo.ItemOrder", "OveralQuantity", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemOrder", "OveralQuantity");
            DropColumn("dbo.ItemLoad", "OveralQuantity");
        }
    }
}
