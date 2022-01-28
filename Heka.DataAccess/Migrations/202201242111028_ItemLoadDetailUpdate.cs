namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadDetailUpdate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ItemOrderDetail", "ItemLoadDetailId", "dbo.ItemLoadDetail");
            DropIndex("dbo.ItemOrderDetail", new[] { "ItemLoadDetailId" });
            AddColumn("dbo.ItemLoad", "CreatDate", c => c.DateTime());
            DropColumn("dbo.ItemOrderDetail", "ItemLoadDetailId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemOrderDetail", "ItemLoadDetailId", c => c.Int());
            DropColumn("dbo.ItemLoad", "CreatDate");
            CreateIndex("dbo.ItemOrderDetail", "ItemLoadDetailId");
            AddForeignKey("dbo.ItemOrderDetail", "ItemLoadDetailId", "dbo.ItemLoadDetail", "Id");
        }
    }
}
