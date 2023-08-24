namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Quantity_DeliveryPlan : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeliveryPlan", "Quantity", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DeliveryPlan", "Quantity");
        }
    }
}
