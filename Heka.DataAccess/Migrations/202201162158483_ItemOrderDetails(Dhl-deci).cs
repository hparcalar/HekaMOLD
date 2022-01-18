namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemOrderDetailsDhldeci : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemOrderDetail", "Desi", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.ItemOrderDetail", "Dhl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemOrderDetail", "Dhl", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.ItemOrderDetail", "Desi");
        }
    }
}
