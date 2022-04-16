namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateDeleteCreatedUserId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ItemLoad", "CreatedUserId", "dbo.User");
            DropIndex("dbo.ItemLoad", new[] { "CreatedUserId" });
            AddColumn("dbo.ItemLoad", "User_Id", c => c.Int());
            CreateIndex("dbo.ItemLoad", "User_Id");
            AddForeignKey("dbo.ItemLoad", "User_Id", "dbo.User", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemLoad", "User_Id", "dbo.User");
            DropIndex("dbo.ItemLoad", new[] { "User_Id" });
            DropColumn("dbo.ItemLoad", "User_Id");
            CreateIndex("dbo.ItemLoad", "CreatedUserId");
            AddForeignKey("dbo.ItemLoad", "CreatedUserId", "dbo.User", "Id");
        }
    }
}
