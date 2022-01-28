namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemOrderUpdateUserAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemOrder", "UserId", c => c.Int());
            CreateIndex("dbo.ItemOrder", "UserId");
            AddForeignKey("dbo.ItemOrder", "UserId", "dbo.User", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemOrder", "UserId", "dbo.User");
            DropIndex("dbo.ItemOrder", new[] { "UserId" });
            DropColumn("dbo.ItemOrder", "UserId");
        }
    }
}
