namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemVariantAdd : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ItemKnitDensity", "ItemId", "dbo.Item");
            DropIndex("dbo.ItemKnitDensity", new[] { "ItemId" });
            CreateTable(
                "dbo.ItemVariant",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemNo = c.String(),
                        ItemName = c.String(),
                        ItemType = c.Int(),
                        ItemCategoryId = c.Int(),
                        ItemGroupId = c.Int(),
                        SupplierFirmId = c.Int(),
                        PlantId = c.Int(),
                        TaxRate = c.Int(),
                        Pattern = c.Int(),
                        CrudeWidth = c.Decimal(precision: 18, scale: 2),
                        CrudeGramaj = c.Decimal(precision: 18, scale: 2),
                        ProductWidth = c.Decimal(precision: 18, scale: 2),
                        ProductGramaj = c.Decimal(precision: 18, scale: 2),
                        WarpWireCount = c.Decimal(precision: 18, scale: 2),
                        MeterGramaj = c.Decimal(precision: 18, scale: 2),
                        ItemCutType = c.Int(),
                        ItemDyeHouseType = c.Int(),
                        ItemApparelType = c.Int(),
                        ItemBulletType = c.Int(),
                        AttemptNo = c.String(),
                        CombWidth = c.Decimal(precision: 18, scale: 2),
                        WeftReportLength = c.Decimal(precision: 18, scale: 2),
                        WarpReportLength = c.Decimal(precision: 18, scale: 2),
                        AverageWeftDensity = c.Int(),
                        AverageWarpDensity = c.Int(),
                        WeavingDraftId = c.Int(),
                        ItemQualityTypeId = c.Int(),
                        ItemId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ItemQualityType", t => t.ItemQualityTypeId)
                .ForeignKey("dbo.WeavingDraft", t => t.WeavingDraftId)
                .ForeignKey("dbo.ItemGroup", t => t.ItemGroupId)
                .ForeignKey("dbo.Item", t => t.ItemId)
                .ForeignKey("dbo.ItemCategory", t => t.ItemCategoryId)
                .ForeignKey("dbo.Firm", t => t.SupplierFirmId)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.ItemCategoryId)
                .Index(t => t.ItemGroupId)
                .Index(t => t.SupplierFirmId)
                .Index(t => t.PlantId)
                .Index(t => t.WeavingDraftId)
                .Index(t => t.ItemQualityTypeId)
                .Index(t => t.ItemId);
            
            AddColumn("dbo.ItemKnitDensity", "ItemVariantId", c => c.Int());
            AddColumn("dbo.ItemUnit", "ItemVariantId", c => c.Int());
            AddColumn("dbo.KnitYarn", "ItemVariantId", c => c.Int());
            AlterColumn("dbo.ItemKnitDensity", "ItemId", c => c.Int());
            CreateIndex("dbo.ItemKnitDensity", "ItemId");
            CreateIndex("dbo.ItemKnitDensity", "ItemVariantId");
            CreateIndex("dbo.ItemUnit", "ItemVariantId");
            CreateIndex("dbo.KnitYarn", "ItemVariantId");
            AddForeignKey("dbo.ItemKnitDensity", "ItemVariantId", "dbo.ItemVariant", "Id");
            AddForeignKey("dbo.ItemUnit", "ItemVariantId", "dbo.ItemVariant", "Id");
            AddForeignKey("dbo.KnitYarn", "ItemVariantId", "dbo.ItemVariant", "Id");
            AddForeignKey("dbo.ItemKnitDensity", "ItemId", "dbo.Item", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemKnitDensity", "ItemId", "dbo.Item");
            DropForeignKey("dbo.ItemVariant", "PlantId", "dbo.Plant");
            DropForeignKey("dbo.ItemVariant", "SupplierFirmId", "dbo.Firm");
            DropForeignKey("dbo.ItemVariant", "ItemCategoryId", "dbo.ItemCategory");
            DropForeignKey("dbo.ItemVariant", "ItemId", "dbo.Item");
            DropForeignKey("dbo.ItemVariant", "ItemGroupId", "dbo.ItemGroup");
            DropForeignKey("dbo.ItemVariant", "WeavingDraftId", "dbo.WeavingDraft");
            DropForeignKey("dbo.KnitYarn", "ItemVariantId", "dbo.ItemVariant");
            DropForeignKey("dbo.ItemUnit", "ItemVariantId", "dbo.ItemVariant");
            DropForeignKey("dbo.ItemVariant", "ItemQualityTypeId", "dbo.ItemQualityType");
            DropForeignKey("dbo.ItemKnitDensity", "ItemVariantId", "dbo.ItemVariant");
            DropIndex("dbo.KnitYarn", new[] { "ItemVariantId" });
            DropIndex("dbo.ItemUnit", new[] { "ItemVariantId" });
            DropIndex("dbo.ItemKnitDensity", new[] { "ItemVariantId" });
            DropIndex("dbo.ItemKnitDensity", new[] { "ItemId" });
            DropIndex("dbo.ItemVariant", new[] { "ItemId" });
            DropIndex("dbo.ItemVariant", new[] { "ItemQualityTypeId" });
            DropIndex("dbo.ItemVariant", new[] { "WeavingDraftId" });
            DropIndex("dbo.ItemVariant", new[] { "PlantId" });
            DropIndex("dbo.ItemVariant", new[] { "SupplierFirmId" });
            DropIndex("dbo.ItemVariant", new[] { "ItemGroupId" });
            DropIndex("dbo.ItemVariant", new[] { "ItemCategoryId" });
            AlterColumn("dbo.ItemKnitDensity", "ItemId", c => c.Int(nullable: false));
            DropColumn("dbo.KnitYarn", "ItemVariantId");
            DropColumn("dbo.ItemUnit", "ItemVariantId");
            DropColumn("dbo.ItemKnitDensity", "ItemVariantId");
            DropTable("dbo.ItemVariant");
            CreateIndex("dbo.ItemKnitDensity", "ItemId");
            AddForeignKey("dbo.ItemKnitDensity", "ItemId", "dbo.Item", "Id", cascadeDelete: true);
        }
    }
}
