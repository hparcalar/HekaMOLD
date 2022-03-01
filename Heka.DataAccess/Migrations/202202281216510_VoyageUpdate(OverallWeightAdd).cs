namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageUpdateOverallWeightAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Voyage", "OverallWeight", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Voyage", "OverallWeight");
        }
    }
}
