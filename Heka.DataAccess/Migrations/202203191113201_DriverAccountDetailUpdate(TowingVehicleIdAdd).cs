namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DriverAccountDetailUpdateTowingVehicleIdAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DriverAccountDetail", "TowingVehicleId", c => c.Int());
            CreateIndex("dbo.DriverAccountDetail", "TowingVehicleId");
            AddForeignKey("dbo.DriverAccountDetail", "TowingVehicleId", "dbo.Vehicle", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DriverAccountDetail", "TowingVehicleId", "dbo.Vehicle");
            DropIndex("dbo.DriverAccountDetail", new[] { "TowingVehicleId" });
            DropColumn("dbo.DriverAccountDetail", "TowingVehicleId");
        }
    }
}
