namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemOrderUpdateOveralVolumeAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemOrder", "OveralWeight", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemOrder", "OveralVolume", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.ItemOrder", "TotalWeight");
            DropColumn("dbo.ItemOrder", "TotalVolume");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemOrder", "TotalVolume", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemOrder", "TotalWeight", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.ItemOrder", "OveralVolume");
            DropColumn("dbo.ItemOrder", "OveralWeight");
        }
    }
}
