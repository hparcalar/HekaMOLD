namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update : DbMigration
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
                        YarnType = c.Int(),
                        ReportWireCount = c.Decimal(precision: 18, scale: 2),
                        MeterWireCount = c.Decimal(precision: 18, scale: 2),
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
            
            CreateTable(
                "dbo.YarnRecipe",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        YarnRecipeCode = c.String(),
                        YarnRecipeName = c.String(),
                        YarnBreedId = c.Int(),
                        Denier = c.Int(),
                        Factor = c.Int(),
                        Twist = c.Int(),
                        CenterType = c.Int(),
                        Mix = c.Boolean(),
                        YarnRecipeType = c.Int(),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                        YarnColourId = c.Int(),
                        FirmId = c.Int(),
                        YarnLot = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Firm", t => t.FirmId)
                .ForeignKey("dbo.YarnBreed", t => t.YarnBreedId)
                .ForeignKey("dbo.YarnColour", t => t.YarnColourId)
                .Index(t => t.YarnBreedId)
                .Index(t => t.YarnColourId)
                .Index(t => t.FirmId);
            
            CreateTable(
                "dbo.YarnBreed",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        YarnBreedCode = c.String(),
                        YarnBreedName = c.String(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.YarnRecipeMix",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        YarnRecipeId = c.Int(nullable: false),
                        YarnBreedId = c.Int(),
                        Percentage = c.Decimal(precision: 18, scale: 2),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.YarnBreed", t => t.YarnBreedId)
                .ForeignKey("dbo.YarnRecipe", t => t.YarnRecipeId, cascadeDelete: true)
                .Index(t => t.YarnRecipeId)
                .Index(t => t.YarnBreedId);
            
            CreateTable(
                "dbo.YarnColour",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        YarnColourCode = c.Int(nullable: false),
                        YarnColourName = c.String(),
                        YarnColourGroupId = c.Int(nullable: false),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.YarnColourGroup", t => t.YarnColourGroupId, cascadeDelete: true)
                .Index(t => t.YarnColourGroupId);
            
            CreateTable(
                "dbo.YarnColourGroup",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        YarnColourGroupCode = c.String(),
                        YarnColourGroupName = c.String(),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Item", "ItemDyeHouseType", c => c.Int());
            AlterColumn("dbo.Item", "CrudeWidth", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Item", "ProductWidth", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Item", "WarpWireCount", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Item", "CombWidth", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Item", "WeftReportLength", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Item", "WarpReportLength", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.Item", "Dyehouse");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Item", "Dyehouse", c => c.String());
            DropForeignKey("dbo.KnitYarn", "ItemId", "dbo.Item");
            DropForeignKey("dbo.YarnRecipeMix", "YarnRecipeId", "dbo.YarnRecipe");
            DropForeignKey("dbo.YarnRecipe", "YarnColourId", "dbo.YarnColour");
            DropForeignKey("dbo.YarnColour", "YarnColourGroupId", "dbo.YarnColourGroup");
            DropForeignKey("dbo.YarnRecipeMix", "YarnBreedId", "dbo.YarnBreed");
            DropForeignKey("dbo.YarnRecipe", "YarnBreedId", "dbo.YarnBreed");
            DropForeignKey("dbo.KnitYarn", "YarnRecipeId", "dbo.YarnRecipe");
            DropForeignKey("dbo.YarnRecipe", "FirmId", "dbo.Firm");
            DropForeignKey("dbo.KnitYarn", "FirmId", "dbo.Firm");
            DropIndex("dbo.YarnColour", new[] { "YarnColourGroupId" });
            DropIndex("dbo.YarnRecipeMix", new[] { "YarnBreedId" });
            DropIndex("dbo.YarnRecipeMix", new[] { "YarnRecipeId" });
            DropIndex("dbo.YarnRecipe", new[] { "FirmId" });
            DropIndex("dbo.YarnRecipe", new[] { "YarnColourId" });
            DropIndex("dbo.YarnRecipe", new[] { "YarnBreedId" });
            DropIndex("dbo.KnitYarn", new[] { "ItemId" });
            DropIndex("dbo.KnitYarn", new[] { "FirmId" });
            DropIndex("dbo.KnitYarn", new[] { "YarnRecipeId" });
            AlterColumn("dbo.Item", "WarpReportLength", c => c.Int());
            AlterColumn("dbo.Item", "WeftReportLength", c => c.Int());
            AlterColumn("dbo.Item", "CombWidth", c => c.Int());
            AlterColumn("dbo.Item", "WarpWireCount", c => c.Int());
            AlterColumn("dbo.Item", "ProductWidth", c => c.Int());
            AlterColumn("dbo.Item", "CrudeWidth", c => c.Int());
            DropColumn("dbo.Item", "ItemDyeHouseType");
            DropTable("dbo.YarnColourGroup");
            DropTable("dbo.YarnColour");
            DropTable("dbo.YarnRecipeMix");
            DropTable("dbo.YarnBreed");
            DropTable("dbo.YarnRecipe");
            DropTable("dbo.KnitYarn");
        }
    }
}
