namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageCostUpdateCreatedIdAndUpdateUserIdAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VoyageCost", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.VoyageCost", "CreatedUserId", c => c.Int());
            AddColumn("dbo.VoyageCost", "UpdatedDate", c => c.DateTime());
            AddColumn("dbo.VoyageCost", "UpdatedUserId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VoyageCost", "UpdatedUserId");
            DropColumn("dbo.VoyageCost", "UpdatedDate");
            DropColumn("dbo.VoyageCost", "CreatedUserId");
            DropColumn("dbo.VoyageCost", "CreatedDate");
        }
    }
}
