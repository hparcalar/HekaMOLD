namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemUpdateintstringTestNo : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Item", "TestNo", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Item", "TestNo", c => c.Int());
        }
    }
}
