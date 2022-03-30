namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemQualityUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemQualityType", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.ItemQualityType", "CreatedUserId", c => c.Int());
            AddColumn("dbo.ItemQualityType", "UpdatedDate", c => c.DateTime());
            AddColumn("dbo.ItemQualityType", "UpdatedUserId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemQualityType", "UpdatedUserId");
            DropColumn("dbo.ItemQualityType", "UpdatedDate");
            DropColumn("dbo.ItemQualityType", "CreatedUserId");
            DropColumn("dbo.ItemQualityType", "CreatedDate");
        }
    }
}
