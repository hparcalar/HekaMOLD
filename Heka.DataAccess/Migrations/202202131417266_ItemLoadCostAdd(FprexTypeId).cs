namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadCostAddFprexTypeId : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.ItemLoadCost", "ForexTypeId");
            AddForeignKey("dbo.ItemLoadCost", "ForexTypeId", "dbo.ForexType", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemLoadCost", "ForexTypeId", "dbo.ForexType");
            DropIndex("dbo.ItemLoadCost", new[] { "ForexTypeId" });
        }
    }
}
