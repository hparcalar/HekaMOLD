namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VehicleInsuranceUpdate : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.VehicleInsurance", name: "UnitId", newName: "UnitTypeId");
            RenameIndex(table: "dbo.VehicleInsurance", name: "IX_UnitId", newName: "IX_UnitTypeId");
            AddColumn("dbo.VehicleInsurance", "Explanation", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VehicleInsurance", "Explanation");
            RenameIndex(table: "dbo.VehicleInsurance", name: "IX_UnitTypeId", newName: "IX_UnitId");
            RenameColumn(table: "dbo.VehicleInsurance", name: "UnitTypeId", newName: "UnitId");
        }
    }
}
