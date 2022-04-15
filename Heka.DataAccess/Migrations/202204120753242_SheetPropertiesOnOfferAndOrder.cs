namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SheetPropertiesOnOfferAndOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemOrderSheet", "SheetWidth", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemOrderSheet", "SheetHeight", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemOfferSheet", "SheetWidth", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemOfferSheet", "SheetHeight", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemOfferSheet", "SheetHeight");
            DropColumn("dbo.ItemOfferSheet", "SheetWidth");
            DropColumn("dbo.ItemOrderSheet", "SheetHeight");
            DropColumn("dbo.ItemOrderSheet", "SheetWidth");
        }
    }
}
