namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateHasCmrDeliveryed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "HasCmrDeliveryed", c => c.Boolean());
            DropColumn("dbo.ItemLoad", "CrmDeliveryDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemLoad", "CrmDeliveryDate", c => c.DateTime());
            DropColumn("dbo.ItemLoad", "HasCmrDeliveryed");
        }
    }
}
