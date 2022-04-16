namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemReceiptUpdateOverallQuantity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemReceipt", "OverallVolume", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemReceipt", "OverallWeight", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemReceipt", "OverallLadametre", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemReceipt", "OverallQuantity", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemReceipt", "OverallQuantity");
            DropColumn("dbo.ItemReceipt", "OverallLadametre");
            DropColumn("dbo.ItemReceipt", "OverallWeight");
            DropColumn("dbo.ItemReceipt", "OverallVolume");
        }
    }
}
