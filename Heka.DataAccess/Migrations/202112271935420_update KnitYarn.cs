namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateKnitYarn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KnitYarn", "YarnType", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.KnitYarn", "YarnType");
        }
    }
}
