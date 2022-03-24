namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemReceiptUpdateLenghtAndWeightAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemReceiptDetail", "ShortWidth", c => c.Int());
            AddColumn("dbo.ItemReceiptDetail", "LongWidth", c => c.Int());
            AddColumn("dbo.ItemReceiptDetail", "Volume", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemReceiptDetail", "Height", c => c.Int());
            AddColumn("dbo.ItemReceiptDetail", "Weight", c => c.Int());
            AddColumn("dbo.ItemReceiptDetail", "Stackable", c => c.Boolean());
            AddColumn("dbo.ItemReceiptDetail", "Ladametre", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemReceiptDetail", "PackageInNumber", c => c.Int());
            AddColumn("dbo.ItemReceiptDetail", "OveralWeight", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemReceiptDetail", "OveralVolume", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemReceiptDetail", "OveralLadametre", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemReceiptDetail", "OverallQuantity", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemReceiptDetail", "OverallQuantity");
            DropColumn("dbo.ItemReceiptDetail", "OveralLadametre");
            DropColumn("dbo.ItemReceiptDetail", "OveralVolume");
            DropColumn("dbo.ItemReceiptDetail", "OveralWeight");
            DropColumn("dbo.ItemReceiptDetail", "PackageInNumber");
            DropColumn("dbo.ItemReceiptDetail", "Ladametre");
            DropColumn("dbo.ItemReceiptDetail", "Stackable");
            DropColumn("dbo.ItemReceiptDetail", "Weight");
            DropColumn("dbo.ItemReceiptDetail", "Height");
            DropColumn("dbo.ItemReceiptDetail", "Volume");
            DropColumn("dbo.ItemReceiptDetail", "LongWidth");
            DropColumn("dbo.ItemReceiptDetail", "ShortWidth");
        }
    }
}
