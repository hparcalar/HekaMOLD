namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigFix : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.ItemOffer", "CreatedUserId");
            AddForeignKey("dbo.ItemOffer", "CreatedUserId", "dbo.User", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemOffer", "CreatedUserId", "dbo.User");
            DropIndex("dbo.ItemOffer", new[] { "CreatedUserId" });
        }
    }
}
