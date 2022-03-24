namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemReceiptUpdateLenghtAndWeightUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemReceiptDetail", "OverallWeight", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemReceiptDetail", "OverallVolume", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemReceiptDetail", "OverallLadametre", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.ItemReceiptDetail", "OveralWeight");
            DropColumn("dbo.ItemReceiptDetail", "OveralVolume");
            DropColumn("dbo.ItemReceiptDetail", "OveralLadametre");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemReceiptDetail", "OveralLadametre", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemReceiptDetail", "OveralVolume", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemReceiptDetail", "OveralWeight", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.ItemReceiptDetail", "OverallLadametre");
            DropColumn("dbo.ItemReceiptDetail", "OverallVolume");
            DropColumn("dbo.ItemReceiptDetail", "OverallWeight");
        }
    }
}
