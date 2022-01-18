namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemOrderUpdateGeneric : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.ItemOrder", name: "FirmId", newName: "CustomerFirmId");
            RenameIndex(table: "dbo.ItemOrder", name: "IX_FirmId", newName: "IX_CustomerFirmId");
            CreateTable(
                "dbo.Customs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomsCode = c.String(),
                        CustomsName = c.String(),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.ItemOrderDetail", "ShortWidth", c => c.Int());
            AddColumn("dbo.ItemOrderDetail", "LongWidth", c => c.Int());
            AddColumn("dbo.ItemOrderDetail", "Volume", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemOrderDetail", "Height", c => c.Int());
            AddColumn("dbo.ItemOrderDetail", "Weight", c => c.Int());
            AddColumn("dbo.ItemOrderDetail", "Stackable", c => c.Boolean());
            AddColumn("dbo.ItemOrderDetail", "Dhl", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemOrderDetail", "CalculationTypeEnum", c => c.Int());
            AddColumn("dbo.ItemOrder", "OrderUploadType", c => c.Int());
            AddColumn("dbo.ItemOrder", "OrderTransactionDirectionType", c => c.Int());
            AddColumn("dbo.ItemOrder", "TotalWeight", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemOrder", "TotalVolume", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemOrder", "Closed", c => c.Boolean());
            AddColumn("dbo.ItemOrder", "EntryCustomsId", c => c.Int());
            CreateIndex("dbo.ItemOrder", "EntryCustomsId");
            AddForeignKey("dbo.ItemOrder", "EntryCustomsId", "dbo.Customs", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemOrder", "EntryCustomsId", "dbo.Customs");
            DropIndex("dbo.ItemOrder", new[] { "EntryCustomsId" });
            DropColumn("dbo.ItemOrder", "EntryCustomsId");
            DropColumn("dbo.ItemOrder", "Closed");
            DropColumn("dbo.ItemOrder", "TotalVolume");
            DropColumn("dbo.ItemOrder", "TotalWeight");
            DropColumn("dbo.ItemOrder", "OrderTransactionDirectionType");
            DropColumn("dbo.ItemOrder", "OrderUploadType");
            DropColumn("dbo.ItemOrderDetail", "CalculationTypeEnum");
            DropColumn("dbo.ItemOrderDetail", "Dhl");
            DropColumn("dbo.ItemOrderDetail", "Stackable");
            DropColumn("dbo.ItemOrderDetail", "Weight");
            DropColumn("dbo.ItemOrderDetail", "Height");
            DropColumn("dbo.ItemOrderDetail", "Volume");
            DropColumn("dbo.ItemOrderDetail", "LongWidth");
            DropColumn("dbo.ItemOrderDetail", "ShortWidth");
            DropTable("dbo.Customs");
            RenameIndex(table: "dbo.ItemOrder", name: "IX_CustomerFirmId", newName: "IX_FirmId");
            RenameColumn(table: "dbo.ItemOrder", name: "CustomerFirmId", newName: "FirmId");
        }
    }
}
