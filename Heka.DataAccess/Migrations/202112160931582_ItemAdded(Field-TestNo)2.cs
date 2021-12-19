namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemAddedFieldTestNo2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Item", "TestNo", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Item", "TestNo", c => c.Int(nullable: false));
        }
    }
}
