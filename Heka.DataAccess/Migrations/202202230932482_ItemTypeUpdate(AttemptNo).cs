namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemTypeUpdateAttemptNo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Item", "AttemptNo", c => c.String());
            DropColumn("dbo.Item", "TestNo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Item", "TestNo", c => c.String());
            DropColumn("dbo.Item", "AttemptNo");
        }
    }
}
