namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemQalityType : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemQualityType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemQualityTypeCode = c.String(),
                        ItemQualityTypeName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Item", "ItemQualityTypeId", c => c.Int());
            CreateIndex("dbo.Item", "ItemQualityTypeId");
            AddForeignKey("dbo.Item", "ItemQualityTypeId", "dbo.ItemQualityType", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Item", "ItemQualityTypeId", "dbo.ItemQualityType");
            DropIndex("dbo.Item", new[] { "ItemQualityTypeId" });
            DropColumn("dbo.Item", "ItemQualityTypeId");
            DropTable("dbo.ItemQualityType");
        }
    }
}
