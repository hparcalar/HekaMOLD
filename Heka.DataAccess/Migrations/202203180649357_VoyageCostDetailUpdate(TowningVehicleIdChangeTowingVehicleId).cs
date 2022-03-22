namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageCostDetailUpdateTowningVehicleIdChangeTowingVehicleId : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.VoyageCostDetail", name: "TowningVehicleId", newName: "TowingVehicleId");
            RenameIndex(table: "dbo.VoyageCostDetail", name: "IX_TowningVehicleId", newName: "IX_TowingVehicleId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.VoyageCostDetail", name: "IX_TowingVehicleId", newName: "IX_TowningVehicleId");
            RenameColumn(table: "dbo.VoyageCostDetail", name: "TowingVehicleId", newName: "TowningVehicleId");
        }
    }
}
