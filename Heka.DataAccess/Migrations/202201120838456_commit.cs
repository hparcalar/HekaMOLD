namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class commit : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.VehicleInsurance", "UnitTypeId", "dbo.UnitType");
            DropIndex("dbo.VehicleInsurance", new[] { "UnitTypeId" });
            AddColumn("dbo.VehicleInsurance", "ForexTypeId", c => c.Int());
            CreateIndex("dbo.VehicleInsurance", "ForexTypeId");
            AddForeignKey("dbo.VehicleInsurance", "ForexTypeId", "dbo.ForexType", "Id");
            DropColumn("dbo.VehicleInsurance", "UnitTypeId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VehicleInsurance", "UnitTypeId", c => c.Int());
            DropForeignKey("dbo.VehicleInsurance", "ForexTypeId", "dbo.ForexType");
            DropIndex("dbo.VehicleInsurance", new[] { "ForexTypeId" });
            DropColumn("dbo.VehicleInsurance", "ForexTypeId");
            CreateIndex("dbo.VehicleInsurance", "UnitTypeId");
            AddForeignKey("dbo.VehicleInsurance", "UnitTypeId", "dbo.UnitType", "Id");
        }
    }
}
