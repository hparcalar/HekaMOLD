namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemReceiptDetail_WeightQuantity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemReceiptDetail", "WeightQuantity", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemReceiptDetail", "WeightQuantity");
        }
    }
}
