namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SheetMultipleUsagesByParts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemOfferSheetUsage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemOfferDetailId = c.Int(),
                        ItemOfferSheetId = c.Int(),
                        Quantity = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ItemOfferSheet", t => t.ItemOfferSheetId)
                .ForeignKey("dbo.ItemOfferDetail", t => t.ItemOfferDetailId)
                .Index(t => t.ItemOfferDetailId)
                .Index(t => t.ItemOfferSheetId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemOfferSheetUsage", "ItemOfferDetailId", "dbo.ItemOfferDetail");
            DropForeignKey("dbo.ItemOfferSheetUsage", "ItemOfferSheetId", "dbo.ItemOfferSheet");
            DropIndex("dbo.ItemOfferSheetUsage", new[] { "ItemOfferSheetId" });
            DropIndex("dbo.ItemOfferSheetUsage", new[] { "ItemOfferDetailId" });
            DropTable("dbo.ItemOfferSheetUsage");
        }
    }
}
