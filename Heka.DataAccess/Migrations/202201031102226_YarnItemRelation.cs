namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class YarnItemRelation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.YarnRecipe", "ItemId", c => c.Int());
            CreateIndex("dbo.YarnRecipe", "ItemId");
            AddForeignKey("dbo.YarnRecipe", "ItemId", "dbo.Item", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.YarnRecipe", "ItemId", "dbo.Item");
            DropIndex("dbo.YarnRecipe", new[] { "ItemId" });
            DropColumn("dbo.YarnRecipe", "ItemId");
        }
    }
}
