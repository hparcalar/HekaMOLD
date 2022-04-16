namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateDeleteCreatDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "CreatedDate", c => c.DateTime());
            DropColumn("dbo.ItemLoad", "CreatDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemLoad", "CreatDate", c => c.DateTime());
            DropColumn("dbo.ItemLoad", "CreatedDate");
        }
    }
}
