namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateYarnBreed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.YarnBreed", "UpdatedUserId", c => c.Int());
            AlterColumn("dbo.YarnBreed", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.YarnBreed", "UpdatedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.YarnBreed", "UpdatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.YarnBreed", "CreatedDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.YarnBreed", "UpdatedUserId");
        }
    }
}
