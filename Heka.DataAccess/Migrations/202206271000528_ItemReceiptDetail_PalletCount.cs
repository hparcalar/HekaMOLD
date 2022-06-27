namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemReceiptDetail_PalletCount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemReceiptDetail", "PalletCount", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemReceiptDetail", "PalletCount");
        }
    }
}
