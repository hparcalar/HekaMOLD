namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VehicleUpdateExplanationAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vehicle", "Explanation", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Vehicle", "Explanation");
        }
    }
}
