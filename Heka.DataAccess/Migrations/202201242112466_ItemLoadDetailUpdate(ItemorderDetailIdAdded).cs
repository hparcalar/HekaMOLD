namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadDetailUpdateItemorderDetailIdAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoadDetail", "ItemOrderDetailId", c => c.Int());
            CreateIndex("dbo.ItemLoadDetail", "ItemOrderDetailId");
            AddForeignKey("dbo.ItemLoadDetail", "ItemOrderDetailId", "dbo.ItemOrderDetail", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemLoadDetail", "ItemOrderDetailId", "dbo.ItemOrderDetail");
            DropIndex("dbo.ItemLoadDetail", new[] { "ItemOrderDetailId" });
            DropColumn("dbo.ItemLoadDetail", "ItemOrderDetailId");
        }
    }
}
