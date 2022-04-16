namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateVoyageIdAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "VoyageId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemLoad", "VoyageId");
        }
    }
}
