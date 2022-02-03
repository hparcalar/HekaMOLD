namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VehicleCareInsuranceTireUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VehicleCare", "DocumentNo", c => c.String());
            AddColumn("dbo.VehicleInsurance", "DocumentNo", c => c.String());
            AddColumn("dbo.VehicleTire", "DocumentNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VehicleTire", "DocumentNo");
            DropColumn("dbo.VehicleInsurance", "DocumentNo");
            DropColumn("dbo.VehicleCare", "DocumentNo");
        }
    }
}
