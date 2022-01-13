namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VehicleTireUpdate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.VehicleTire", "UnitId", "dbo.UnitType");
            DropIndex("dbo.VehicleTire", new[] { "UnitId" });
            AddColumn("dbo.VehicleTire", "VehicleTireType", c => c.Int());
            AddColumn("dbo.VehicleTire", "ForexTypeId", c => c.Int());
            AddColumn("dbo.VehicleTire", "Explanation", c => c.String());
            CreateIndex("dbo.VehicleTire", "ForexTypeId");
            AddForeignKey("dbo.VehicleTire", "ForexTypeId", "dbo.ForexType", "Id");
            DropColumn("dbo.VehicleTire", "VehicleTireTyp");
            DropColumn("dbo.VehicleTire", "KmHourControl");
            DropColumn("dbo.VehicleTire", "UnitId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VehicleTire", "UnitId", c => c.Int());
            AddColumn("dbo.VehicleTire", "KmHourControl", c => c.Boolean());
            AddColumn("dbo.VehicleTire", "VehicleTireTyp", c => c.Int());
            DropForeignKey("dbo.VehicleTire", "ForexTypeId", "dbo.ForexType");
            DropIndex("dbo.VehicleTire", new[] { "ForexTypeId" });
            DropColumn("dbo.VehicleTire", "Explanation");
            DropColumn("dbo.VehicleTire", "ForexTypeId");
            DropColumn("dbo.VehicleTire", "VehicleTireType");
            CreateIndex("dbo.VehicleTire", "UnitId");
            AddForeignKey("dbo.VehicleTire", "UnitId", "dbo.UnitType", "Id");
        }
    }
}
