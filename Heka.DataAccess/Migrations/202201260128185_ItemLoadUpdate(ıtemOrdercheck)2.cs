namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateıtemOrdercheck2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "DateOfNeed", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemLoad", "DateOfNeed");
        }
    }
}
