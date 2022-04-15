namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ThicknessToDecimal : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ItemOrderSheet", "Thickness", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.ItemOfferDetail", "SheetTickness", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.ItemOfferSheet", "Thickness", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ItemOfferSheet", "Thickness", c => c.Int());
            AlterColumn("dbo.ItemOfferDetail", "SheetTickness", c => c.Int());
            AlterColumn("dbo.ItemOrderSheet", "Thickness", c => c.Int());
        }
    }
}
