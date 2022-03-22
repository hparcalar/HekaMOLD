namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageCostDetailUpdateOperationTypeAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VoyageCostDetail", "ActionType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VoyageCostDetail", "ActionType");
        }
    }
}
