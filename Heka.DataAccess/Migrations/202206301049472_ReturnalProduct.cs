namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReturnalProduct : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReturnalProduct",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReturnCode = c.String(),
                        ReturnDate = c.DateTime(),
                        Explanation = c.String(),
                        PlantId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.PlantId);
            
            CreateTable(
                "dbo.ReturnalProductDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReturnalProductId = c.Int(),
                        ItemId = c.Int(),
                        Quantity = c.Decimal(precision: 18, scale: 2),
                        PackageQuantity = c.Decimal(precision: 18, scale: 2),
                        PalletQuantity = c.Decimal(precision: 18, scale: 2),
                        Explanation = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Item", t => t.ItemId)
                .ForeignKey("dbo.ReturnalProduct", t => t.ReturnalProductId)
                .Index(t => t.ReturnalProductId)
                .Index(t => t.ItemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReturnalProductDetail", "ReturnalProductId", "dbo.ReturnalProduct");
            DropForeignKey("dbo.ReturnalProductDetail", "ItemId", "dbo.Item");
            DropForeignKey("dbo.ReturnalProduct", "PlantId", "dbo.Plant");
            DropIndex("dbo.ReturnalProductDetail", new[] { "ItemId" });
            DropIndex("dbo.ReturnalProductDetail", new[] { "ReturnalProductId" });
            DropIndex("dbo.ReturnalProduct", new[] { "PlantId" });
            DropTable("dbo.ReturnalProductDetail");
            DropTable("dbo.ReturnalProduct");
        }
    }
}
