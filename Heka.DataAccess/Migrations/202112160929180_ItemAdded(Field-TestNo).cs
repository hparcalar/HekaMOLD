namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemAddedFieldTestNo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Item", "TestNo", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Item", "TestNo");
        }
    }
}
