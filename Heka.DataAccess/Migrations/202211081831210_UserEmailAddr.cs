namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserEmailAddr : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "EmailAddr", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "EmailAddr");
        }
    }
}
