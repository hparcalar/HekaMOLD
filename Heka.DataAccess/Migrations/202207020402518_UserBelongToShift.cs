namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserBelongToShift : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "ShiftId", c => c.Int());
            CreateIndex("dbo.User", "ShiftId");
            AddForeignKey("dbo.User", "ShiftId", "dbo.Shift", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.User", "ShiftId", "dbo.Shift");
            DropIndex("dbo.User", new[] { "ShiftId" });
            DropColumn("dbo.User", "ShiftId");
        }
    }
}
