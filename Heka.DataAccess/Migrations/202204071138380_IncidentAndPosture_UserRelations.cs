namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IncidentAndPosture_UserRelations : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Incident", "CreatedUserId");
            CreateIndex("dbo.Incident", "StartedUserId");
            CreateIndex("dbo.Incident", "EndUserId");
            CreateIndex("dbo.ProductionPosture", "CreatedUserId");
            CreateIndex("dbo.ProductionPosture", "UpdatedUserId");
            AddForeignKey("dbo.Incident", "CreatedUserId", "dbo.User", "Id");
            AddForeignKey("dbo.Incident", "EndUserId", "dbo.User", "Id");
            AddForeignKey("dbo.Incident", "StartedUserId", "dbo.User", "Id");
            AddForeignKey("dbo.ProductionPosture", "CreatedUserId", "dbo.User", "Id");
            AddForeignKey("dbo.ProductionPosture", "UpdatedUserId", "dbo.User", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductionPosture", "UpdatedUserId", "dbo.User");
            DropForeignKey("dbo.ProductionPosture", "CreatedUserId", "dbo.User");
            DropForeignKey("dbo.Incident", "StartedUserId", "dbo.User");
            DropForeignKey("dbo.Incident", "EndUserId", "dbo.User");
            DropForeignKey("dbo.Incident", "CreatedUserId", "dbo.User");
            DropIndex("dbo.ProductionPosture", new[] { "UpdatedUserId" });
            DropIndex("dbo.ProductionPosture", new[] { "CreatedUserId" });
            DropIndex("dbo.Incident", new[] { "EndUserId" });
            DropIndex("dbo.Incident", new[] { "StartedUserId" });
            DropIndex("dbo.Incident", new[] { "CreatedUserId" });
        }
    }
}
