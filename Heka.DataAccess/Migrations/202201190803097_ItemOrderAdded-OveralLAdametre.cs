namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemOrderAddedOveralLAdametre : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemOrder", "OveralLadametre", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.ItemOrderDetail", "Ladametre", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ItemOrderDetail", "Ladametre", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.ItemOrder", "OveralLadametre");
        }
    }
}
