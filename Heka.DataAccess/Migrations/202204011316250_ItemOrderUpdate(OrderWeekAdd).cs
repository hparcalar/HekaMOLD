namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemOrderUpdateOrderWeekAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemOrder", "OrderWeek", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemOrder", "OrderWeek");
        }
    }
}
