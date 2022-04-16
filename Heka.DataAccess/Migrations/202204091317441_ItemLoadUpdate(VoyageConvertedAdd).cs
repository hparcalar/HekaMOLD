namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateVoyageConvertedAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "VoyageConverted", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemLoad", "VoyageConverted");
        }
    }
}
