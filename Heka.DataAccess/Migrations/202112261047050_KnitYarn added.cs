namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class KnitYarnadded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.KnitYarn",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        YarnRecipeId = c.Int(nullable: false),
                        FirmId = c.Int(nullable: false),
                        ItemId = c.Int(nullable: false),
                        ReportWireCount = c.Int(),
                        MeterWireCount = c.Int(),
                        Gramaj = c.Decimal(precision: 18, scale: 2),
                        Density = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Firm", t => t.FirmId, cascadeDelete: true)
                .ForeignKey("dbo.YarnRecipe", t => t.YarnRecipeId, cascadeDelete: true)
                .ForeignKey("dbo.Item", t => t.ItemId, cascadeDelete: true)
                .Index(t => t.YarnRecipeId)
                .Index(t => t.FirmId)
                .Index(t => t.ItemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.KnitYarn", "ItemId", "dbo.Item");
            DropForeignKey("dbo.KnitYarn", "YarnRecipeId", "dbo.YarnRecipe");
            DropForeignKey("dbo.KnitYarn", "FirmId", "dbo.Firm");
            DropIndex("dbo.KnitYarn", new[] { "ItemId" });
            DropIndex("dbo.KnitYarn", new[] { "FirmId" });
            DropIndex("dbo.KnitYarn", new[] { "YarnRecipeId" });
            DropTable("dbo.KnitYarn");
        }
    }
}
