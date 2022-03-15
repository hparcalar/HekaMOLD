namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeliveryPlan_ItemOrderDetail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeliveryPlan", "ItemOrderDetailId", c => c.Int());
            CreateIndex("dbo.DeliveryPlan", "ItemOrderDetailId");
            AddForeignKey("dbo.DeliveryPlan", "ItemOrderDetailId", "dbo.ItemOrderDetail", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DeliveryPlan", "ItemOrderDetailId", "dbo.ItemOrderDetail");
            DropIndex("dbo.DeliveryPlan", new[] { "ItemOrderDetailId" });
            DropColumn("dbo.DeliveryPlan", "ItemOrderDetailId");
        }
    }
}
