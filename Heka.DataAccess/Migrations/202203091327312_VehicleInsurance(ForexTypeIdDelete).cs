namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VehicleInsuranceForexTypeIdDelete : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.VehicleInsurance", "ForexTypeId", "dbo.ForexType");
            DropIndex("dbo.VehicleInsurance", new[] { "ForexTypeId" });
            DropColumn("dbo.VehicleInsurance", "ForexTypeId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VehicleInsurance", "ForexTypeId", c => c.Int());
            CreateIndex("dbo.VehicleInsurance", "ForexTypeId");
            AddForeignKey("dbo.VehicleInsurance", "ForexTypeId", "dbo.ForexType", "Id");
        }
    }
}
