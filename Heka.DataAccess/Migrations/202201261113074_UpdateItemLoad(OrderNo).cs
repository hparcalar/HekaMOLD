namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateItemLoadOrderNo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "OrderNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemLoad", "OrderNo");
        }
    }
}
