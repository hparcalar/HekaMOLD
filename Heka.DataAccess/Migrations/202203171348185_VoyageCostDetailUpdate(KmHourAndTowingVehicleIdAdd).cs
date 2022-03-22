namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageCostDetailUpdateKmHourAndTowingVehicleIdAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VoyageCostDetail", "TowningVehicleId", c => c.Int());
            AddColumn("dbo.VoyageCostDetail", "KmHour", c => c.Int());
            CreateIndex("dbo.VoyageCostDetail", "TowningVehicleId");
            AddForeignKey("dbo.VoyageCostDetail", "TowningVehicleId", "dbo.Vehicle", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VoyageCostDetail", "TowningVehicleId", "dbo.Vehicle");
            DropIndex("dbo.VoyageCostDetail", new[] { "TowningVehicleId" });
            DropColumn("dbo.VoyageCostDetail", "KmHour");
            DropColumn("dbo.VoyageCostDetail", "TowningVehicleId");
        }
    }
}
