namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoad_Voyage_Relation : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.ItemLoad", "VoyageId");
            AddForeignKey("dbo.ItemLoad", "VoyageId", "dbo.Voyage", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemLoad", "VoyageId", "dbo.Voyage");
            DropIndex("dbo.ItemLoad", new[] { "VoyageId" });
        }
    }
}
