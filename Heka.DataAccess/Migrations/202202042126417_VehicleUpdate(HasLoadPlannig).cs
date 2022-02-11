namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VehicleUpdateHasLoadPlannig : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vehicle", "HasLoadPlannig", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Vehicle", "HasLoadPlannig");
        }
    }
}
