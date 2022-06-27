namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemQualityGroupAndItemSheetData : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemQualityGroup",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemQualityGroupCode = c.String(),
                        ItemQualityGroupName = c.String(),
                        PlantId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.PlantId);
            
            AddColumn("dbo.Item", "SheetWidth", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Item", "SheetHeight", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Item", "SheetThickness", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Item", "SheetUnitWeight", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Item", "ItemQualityGroupId", c => c.Int());
            CreateIndex("dbo.Item", "ItemQualityGroupId");
            AddForeignKey("dbo.Item", "ItemQualityGroupId", "dbo.ItemQualityGroup", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Item", "ItemQualityGroupId", "dbo.ItemQualityGroup");
            DropForeignKey("dbo.ItemQualityGroup", "PlantId", "dbo.Plant");
            DropIndex("dbo.ItemQualityGroup", new[] { "PlantId" });
            DropIndex("dbo.Item", new[] { "ItemQualityGroupId" });
            DropColumn("dbo.Item", "ItemQualityGroupId");
            DropColumn("dbo.Item", "SheetUnitWeight");
            DropColumn("dbo.Item", "SheetThickness");
            DropColumn("dbo.Item", "SheetHeight");
            DropColumn("dbo.Item", "SheetWidth");
            DropTable("dbo.ItemQualityGroup");
        }
    }
}
