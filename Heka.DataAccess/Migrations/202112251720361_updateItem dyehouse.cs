namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateItemdyehouse : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Item", "ItemDyeHouseType", c => c.Int());
            DropColumn("dbo.Item", "Dyehouse");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Item", "Dyehouse", c => c.String());
            DropColumn("dbo.Item", "ItemDyeHouseType");
        }
    }
}
