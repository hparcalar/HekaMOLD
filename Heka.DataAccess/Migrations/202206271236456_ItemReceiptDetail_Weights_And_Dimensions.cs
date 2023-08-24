namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemReceiptDetail_Weights_And_Dimensions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemReceiptDetail", "NetWeight", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemReceiptDetail", "GrossWeight", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemReceiptDetail", "PackageDimension", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemReceiptDetail", "PackageDimension");
            DropColumn("dbo.ItemReceiptDetail", "GrossWeight");
            DropColumn("dbo.ItemReceiptDetail", "NetWeight");
        }
    }
}
