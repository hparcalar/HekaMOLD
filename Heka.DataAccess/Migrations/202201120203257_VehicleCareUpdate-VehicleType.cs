namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VehicleCareUpdateVehicleType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VehicleCare", "VehicleTireType", c => c.Int());
            DropColumn("dbo.VehicleCare", "VehiceTireType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VehicleCare", "VehiceTireType", c => c.Int());
            DropColumn("dbo.VehicleCare", "VehicleTireType");
        }
    }
}
