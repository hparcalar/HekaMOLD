namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OfferAndOrderSheetItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemOrderSheet", "SheetItemId", c => c.Int());
            AddColumn("dbo.ItemOfferSheet", "SheetItemId", c => c.Int());
            CreateIndex("dbo.ItemOrderSheet", "SheetItemId");
            CreateIndex("dbo.ItemOfferSheet", "SheetItemId");
            AddForeignKey("dbo.ItemOrderSheet", "SheetItemId", "dbo.Item", "Id");
            AddForeignKey("dbo.ItemOfferSheet", "SheetItemId", "dbo.Item", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemOfferSheet", "SheetItemId", "dbo.Item");
            DropForeignKey("dbo.ItemOrderSheet", "SheetItemId", "dbo.Item");
            DropIndex("dbo.ItemOfferSheet", new[] { "SheetItemId" });
            DropIndex("dbo.ItemOrderSheet", new[] { "SheetItemId" });
            DropColumn("dbo.ItemOfferSheet", "SheetItemId");
            DropColumn("dbo.ItemOrderSheet", "SheetItemId");
        }
    }
}
