namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemOrderUpdateCalculationTypePrice2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ItemOrder", "CalculationTypePrice", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ItemOrder", "CalculationTypePrice", c => c.Int());
        }
    }
}
