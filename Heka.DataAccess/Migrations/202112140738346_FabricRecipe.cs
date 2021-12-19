namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FabricRecipe : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.YarnRecipe", "ItemId", "dbo.Item");
            DropForeignKey("dbo.YarnRecipe", "YarnRecipeTypeId", "dbo.YarnRecipeType");
            DropForeignKey("dbo.ItemKnitDensity", "YarnRecipeTypeId", "dbo.YarnRecipeType");
            DropIndex("dbo.ItemKnitDensity", new[] { "YarnRecipeTypeId" });
            DropIndex("dbo.YarnRecipe", new[] { "ItemId" });
            DropIndex("dbo.YarnRecipe", new[] { "YarnRecipeTypeId" });
            CreateTable(
                "dbo.FabricRecipe",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemId = c.Int(),
                        FabricRecipeCode = c.String(),
                        FabricRecipeName = c.String(),
                        Denier = c.Int(),
                        FirmId = c.Int(),
                        YarnRecipeType = c.String(),
                        ReportWireCount = c.Int(),
                        MeterWireCount = c.Int(),
                        Gramaj = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Firm", t => t.FirmId)
                .ForeignKey("dbo.Item", t => t.ItemId)
                .Index(t => t.ItemId)
                .Index(t => t.FirmId);
            
            AddColumn("dbo.ItemKnitDensity", "YarnRecipeType", c => c.String());
            DropColumn("dbo.ItemKnitDensity", "YarnRecipeTypeId");
            DropColumn("dbo.YarnRecipe", "ItemId");
            DropColumn("dbo.YarnRecipe", "YarnRecipeTypeId");
            DropColumn("dbo.YarnRecipe", "ReportWireCount");
            DropColumn("dbo.YarnRecipe", "MeterWireCount");
            DropColumn("dbo.YarnRecipe", "Gramaj");
            DropTable("dbo.YarnRecipeType");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.YarnRecipeType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        YarnTypeCode = c.String(),
                        YarnTypeName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.YarnRecipe", "Gramaj", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.YarnRecipe", "MeterWireCount", c => c.Int());
            AddColumn("dbo.YarnRecipe", "ReportWireCount", c => c.Int());
            AddColumn("dbo.YarnRecipe", "YarnRecipeTypeId", c => c.Int(nullable: false));
            AddColumn("dbo.YarnRecipe", "ItemId", c => c.Int());
            AddColumn("dbo.ItemKnitDensity", "YarnRecipeTypeId", c => c.Int(nullable: false));
            DropForeignKey("dbo.FabricRecipe", "ItemId", "dbo.Item");
            DropForeignKey("dbo.FabricRecipe", "FirmId", "dbo.Firm");
            DropIndex("dbo.FabricRecipe", new[] { "FirmId" });
            DropIndex("dbo.FabricRecipe", new[] { "ItemId" });
            DropColumn("dbo.ItemKnitDensity", "YarnRecipeType");
            DropTable("dbo.FabricRecipe");
            CreateIndex("dbo.YarnRecipe", "YarnRecipeTypeId");
            CreateIndex("dbo.YarnRecipe", "ItemId");
            CreateIndex("dbo.ItemKnitDensity", "YarnRecipeTypeId");
            AddForeignKey("dbo.ItemKnitDensity", "YarnRecipeTypeId", "dbo.YarnRecipeType", "Id", cascadeDelete: true);
            AddForeignKey("dbo.YarnRecipe", "YarnRecipeTypeId", "dbo.YarnRecipeType", "Id", cascadeDelete: true);
            AddForeignKey("dbo.YarnRecipe", "ItemId", "dbo.Item", "Id");
        }
    }
}
