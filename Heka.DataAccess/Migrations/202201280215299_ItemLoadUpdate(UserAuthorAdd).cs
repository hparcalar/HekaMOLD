namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateUserAuthorAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "UserAuthorId", c => c.Int());
            CreateIndex("dbo.ItemLoad", "UserAuthorId");
            AddForeignKey("dbo.ItemLoad", "UserAuthorId", "dbo.User", "Id");
            DropColumn("dbo.ItemLoad", "LoadAuthor");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemLoad", "LoadAuthor", c => c.String());
            DropForeignKey("dbo.ItemLoad", "UserAuthorId", "dbo.User");
            DropIndex("dbo.ItemLoad", new[] { "UserAuthorId" });
            DropColumn("dbo.ItemLoad", "UserAuthorId");
        }
    }
}
