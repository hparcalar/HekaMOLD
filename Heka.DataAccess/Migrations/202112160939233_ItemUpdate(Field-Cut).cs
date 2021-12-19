namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemUpdateFieldCut : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Item", "Cut", c => c.String());
            DropColumn("dbo.Item", "Cutting");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Item", "Cutting", c => c.String());
            DropColumn("dbo.Item", "Cut");
        }
    }
}
