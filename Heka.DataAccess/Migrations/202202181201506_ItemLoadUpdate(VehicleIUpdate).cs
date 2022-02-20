namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateVehicleIUpdate : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.ItemLoad", name: "VehicleId", newName: "VehicleTraillerId");
            RenameIndex(table: "dbo.ItemLoad", name: "IX_VehicleId", newName: "IX_VehicleTraillerId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.ItemLoad", name: "IX_VehicleTraillerId", newName: "IX_VehicleId");
            RenameColumn(table: "dbo.ItemLoad", name: "VehicleTraillerId", newName: "VehicleId");
        }
    }
}
