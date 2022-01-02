namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateYarncolourModel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.YarnColour", "YarnColourCode", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.YarnColour", "YarnColourCode", c => c.String());
        }
    }
}
