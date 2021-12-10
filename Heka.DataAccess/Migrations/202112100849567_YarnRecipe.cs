namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class YarnRecipe : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.YarnRecipe",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemId = c.Int(),
                        YarnRecipeCode = c.String(),
                        YarnRecipeName = c.String(),
                        FirmId = c.Int(),
                        YarnRecipeTypeId = c.Int(nullable: false),
                        Denier = c.Int(),
                        ReportWireCount = c.Int(),
                        MeterWireCount = c.Int(),
                        Gramaj = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Firm", t => t.FirmId)
                .ForeignKey("dbo.Item", t => t.ItemId)
                .Index(t => t.ItemId)
                .Index(t => t.FirmId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.YarnRecipe", "ItemId", "dbo.Item");
            DropForeignKey("dbo.YarnRecipe", "FirmId", "dbo.Firm");
            DropIndex("dbo.YarnRecipe", new[] { "FirmId" });
            DropIndex("dbo.YarnRecipe", new[] { "ItemId" });
            DropTable("dbo.YarnRecipe");
        }
    }
}
