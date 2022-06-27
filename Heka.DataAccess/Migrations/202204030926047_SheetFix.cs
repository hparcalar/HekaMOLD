namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SheetFix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ItemOfferSheet", "ItemOfferDetailId", "dbo.ItemOfferDetail");
            DropForeignKey("dbo.ItemOrderSheet", "ItemOrderDetailId", "dbo.ItemOrderDetail");
            DropIndex("dbo.ItemOfferSheet", new[] { "ItemOfferDetailId" });
            DropIndex("dbo.ItemOrderSheet", new[] { "ItemOrderDetailId" });
            AddColumn("dbo.ItemOrderDetail", "ItemOrderSheetId", c => c.Int());
            AddColumn("dbo.ItemOfferDetail", "ItemOfferSheetId", c => c.Int());
            CreateIndex("dbo.ItemOrderDetail", "ItemOrderSheetId");
            CreateIndex("dbo.ItemOfferDetail", "ItemOfferSheetId");
            AddForeignKey("dbo.ItemOfferDetail", "ItemOfferSheetId", "dbo.ItemOfferSheet", "Id");
            AddForeignKey("dbo.ItemOrderDetail", "ItemOrderSheetId", "dbo.ItemOrderSheet", "Id");
            DropColumn("dbo.ItemOfferSheet", "ItemOfferDetailId");
            DropColumn("dbo.ItemOrderSheet", "ItemOrderDetailId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemOrderSheet", "ItemOrderDetailId", c => c.Int());
            AddColumn("dbo.ItemOfferSheet", "ItemOfferDetailId", c => c.Int());
            DropForeignKey("dbo.ItemOrderDetail", "ItemOrderSheetId", "dbo.ItemOrderSheet");
            DropForeignKey("dbo.ItemOfferDetail", "ItemOfferSheetId", "dbo.ItemOfferSheet");
            DropIndex("dbo.ItemOfferDetail", new[] { "ItemOfferSheetId" });
            DropIndex("dbo.ItemOrderDetail", new[] { "ItemOrderSheetId" });
            DropColumn("dbo.ItemOfferDetail", "ItemOfferSheetId");
            DropColumn("dbo.ItemOrderDetail", "ItemOrderSheetId");
            CreateIndex("dbo.ItemOrderSheet", "ItemOrderDetailId");
            CreateIndex("dbo.ItemOfferSheet", "ItemOfferDetailId");
            AddForeignKey("dbo.ItemOrderSheet", "ItemOrderDetailId", "dbo.ItemOrderDetail", "Id");
            AddForeignKey("dbo.ItemOfferSheet", "ItemOfferDetailId", "dbo.ItemOfferDetail", "Id");
        }
    }
}
