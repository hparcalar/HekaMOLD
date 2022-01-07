namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VehicleUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vehicle", "Price", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.Vehicle", "Amount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Vehicle", "Amount", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.Vehicle", "Price");
        }
    }
}
