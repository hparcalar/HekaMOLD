namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemPricesAndItemTaxDefinition : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemPrice",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemId = c.Int(),
                        ForexTypeId = c.Int(),
                        Price = c.Decimal(precision: 18, scale: 2),
                        PriceType = c.Int(),
                        IsDefault = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ForexType", t => t.ForexTypeId)
                .ForeignKey("dbo.Item", t => t.ItemId)
                .Index(t => t.ItemId)
                .Index(t => t.ForexTypeId);
            
            AddColumn("dbo.Item", "TaxRate", c => c.Int());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemPrice", "ItemId", "dbo.Item");
            DropForeignKey("dbo.ItemPrice", "ForexTypeId", "dbo.ForexType");
            DropIndex("dbo.ItemPrice", new[] { "ForexTypeId" });
            DropIndex("dbo.ItemPrice", new[] { "ItemId" });
            DropColumn("dbo.Item", "TaxRate");
            DropTable("dbo.ItemPrice");
        }
    }
}
