namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateDeliveryDateAdd2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "LoadExitDate", c => c.DateTime());
            AddColumn("dbo.ItemLoad", "DeliveryDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemLoad", "DeliveryDate");
            DropColumn("dbo.ItemLoad", "LoadExitDate");
        }
    }
}
