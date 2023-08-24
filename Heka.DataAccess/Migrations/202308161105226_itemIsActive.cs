namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class itemIsActive : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Item", "isActive", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Item", "isActive");
        }
    }
}
