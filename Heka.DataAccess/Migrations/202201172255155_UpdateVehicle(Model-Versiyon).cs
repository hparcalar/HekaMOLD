namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateVehicleModelVersiyon : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vehicle", "Versiyon", c => c.String());
            DropColumn("dbo.Vehicle", "Model");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Vehicle", "Model", c => c.String());
            DropColumn("dbo.Vehicle", "Versiyon");
        }
    }
}
