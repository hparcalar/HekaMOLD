namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageUpdateCustomsDoorAdd : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Voyage", "CustomsDoorEntryId");
            CreateIndex("dbo.Voyage", "CustomsDoorExitId");
            AddForeignKey("dbo.Voyage", "CustomsDoorEntryId", "dbo.CustomsDoor", "Id");
            AddForeignKey("dbo.Voyage", "CustomsDoorExitId", "dbo.CustomsDoor", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Voyage", "CustomsDoorExitId", "dbo.CustomsDoor");
            DropForeignKey("dbo.Voyage", "CustomsDoorEntryId", "dbo.CustomsDoor");
            DropIndex("dbo.Voyage", new[] { "CustomsDoorExitId" });
            DropIndex("dbo.Voyage", new[] { "CustomsDoorEntryId" });
        }
    }
}
