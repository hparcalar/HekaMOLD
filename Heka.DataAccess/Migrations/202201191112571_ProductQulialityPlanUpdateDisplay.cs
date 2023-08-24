namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductQulialityPlanUpdateDisplay : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductQualityPlan", "Display", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductQualityPlan", "Display");
        }
    }
}
