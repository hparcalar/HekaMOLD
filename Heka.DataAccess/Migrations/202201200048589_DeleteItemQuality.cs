namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteItemQuality : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Item", "ItemQualityTypeId", "dbo.ItemQualityType");
            DropIndex("dbo.Item", new[] { "ItemQualityTypeId" });
            DropColumn("dbo.Item", "ItemQualityTypeId");
            DropTable("dbo.ItemQualityType");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ItemQualityType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemQualityTypeCode = c.String(),
                        ItemQualityTypeName = c.String(),
                        IsActive = c.Boolean(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Item", "ItemQualityTypeId", c => c.Int());
            CreateIndex("dbo.Item", "ItemQualityTypeId");
            AddForeignKey("dbo.Item", "ItemQualityTypeId", "dbo.ItemQualityType", "Id");
        }
    }
}
