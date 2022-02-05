namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemReceipt_PriceCalcType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemReceipt", "PriceCalcType", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemReceipt", "PriceCalcType");
        }
    }
}
