namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserShortcuts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserShortcut",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(),
                        Path = c.String(),
                        Title = c.String(),
                        Explanation = c.String(),
                        PathParams = c.String(),
                        X = c.Int(),
                        Y = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserShortcut", "UserId", "dbo.User");
            DropIndex("dbo.UserShortcut", new[] { "UserId" });
            DropTable("dbo.UserShortcut");
        }
    }
}
