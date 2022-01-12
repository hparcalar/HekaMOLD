namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VehicleCareUpdateGenel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.VehicleCare", "UnitId", "dbo.UnitType");
            DropIndex("dbo.VehicleCare", new[] { "UnitId" });
            AddColumn("dbo.VehicleCare", "PersonnelId", c => c.Int());
            AddColumn("dbo.VehicleCare", "CareDate", c => c.DateTime());
            AddColumn("dbo.VehicleCare", "ForexTypeId", c => c.Int());
            AddColumn("dbo.VehicleCare", "Explanation", c => c.String());
            CreateIndex("dbo.VehicleCare", "ForexTypeId");
            AddForeignKey("dbo.VehicleCare", "ForexTypeId", "dbo.ForexType", "Id");
            DropColumn("dbo.VehicleCare", "VehicleTireType");
            DropColumn("dbo.VehicleCare", "SeriNo");
            DropColumn("dbo.VehicleCare", "DirectionType");
            DropColumn("dbo.VehicleCare", "DimensionsInfo");
            DropColumn("dbo.VehicleCare", "MontageDate");
            DropColumn("dbo.VehicleCare", "KmHourLimit");
            DropColumn("dbo.VehicleCare", "KmHourControl");
            DropColumn("dbo.VehicleCare", "UnitId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VehicleCare", "UnitId", c => c.Int());
            AddColumn("dbo.VehicleCare", "KmHourControl", c => c.Boolean());
            AddColumn("dbo.VehicleCare", "KmHourLimit", c => c.Int());
            AddColumn("dbo.VehicleCare", "MontageDate", c => c.DateTime());
            AddColumn("dbo.VehicleCare", "DimensionsInfo", c => c.String());
            AddColumn("dbo.VehicleCare", "DirectionType", c => c.Int());
            AddColumn("dbo.VehicleCare", "SeriNo", c => c.String());
            AddColumn("dbo.VehicleCare", "VehicleTireType", c => c.Int());
            DropForeignKey("dbo.VehicleCare", "ForexTypeId", "dbo.ForexType");
            DropIndex("dbo.VehicleCare", new[] { "ForexTypeId" });
            DropColumn("dbo.VehicleCare", "Explanation");
            DropColumn("dbo.VehicleCare", "ForexTypeId");
            DropColumn("dbo.VehicleCare", "CareDate");
            DropColumn("dbo.VehicleCare", "PersonnelId");
            CreateIndex("dbo.VehicleCare", "UnitId");
            AddForeignKey("dbo.VehicleCare", "UnitId", "dbo.UnitType", "Id");
        }
    }
}
