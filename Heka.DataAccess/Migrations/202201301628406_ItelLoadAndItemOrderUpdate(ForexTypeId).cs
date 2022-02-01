namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItelLoadAndItemOrderUpdateForexTypeId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "ForexTypeId", c => c.Int());
            AddColumn("dbo.ItemOrder", "ForexTypeId", c => c.Int());
            CreateIndex("dbo.ItemLoad", "ForexTypeId");
            CreateIndex("dbo.ItemOrder", "ForexTypeId");
            AddForeignKey("dbo.ItemLoad", "ForexTypeId", "dbo.ForexType", "Id");
            AddForeignKey("dbo.ItemOrder", "ForexTypeId", "dbo.ForexType", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemOrder", "ForexTypeId", "dbo.ForexType");
            DropForeignKey("dbo.ItemLoad", "ForexTypeId", "dbo.ForexType");
            DropIndex("dbo.ItemOrder", new[] { "ForexTypeId" });
            DropIndex("dbo.ItemLoad", new[] { "ForexTypeId" });
            DropColumn("dbo.ItemOrder", "ForexTypeId");
            DropColumn("dbo.ItemLoad", "ForexTypeId");
        }
    }
}
