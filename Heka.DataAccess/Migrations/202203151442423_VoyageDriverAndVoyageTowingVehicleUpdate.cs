namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageDriverAndVoyageTowingVehicleUpdate : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.VoyageDriver", name: "TowinfVehicleId", newName: "TowingVehicleId");
            RenameIndex(table: "dbo.VoyageDriver", name: "IX_TowinfVehicleId", newName: "IX_TowingVehicleId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.VoyageDriver", name: "IX_TowingVehicleId", newName: "IX_TowinfVehicleId");
            RenameColumn(table: "dbo.VoyageDriver", name: "TowingVehicleId", newName: "TowinfVehicleId");
        }
    }
}
