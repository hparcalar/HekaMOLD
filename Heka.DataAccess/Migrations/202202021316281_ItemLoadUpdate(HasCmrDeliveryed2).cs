namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateHasCmrDeliveryed2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "CmrNo", c => c.String());
            AddColumn("dbo.ItemLoad", "CmrStatus", c => c.Int());
            DropColumn("dbo.ItemLoad", "CrmNo");
            DropColumn("dbo.ItemLoad", "CrmStatus");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemLoad", "CrmStatus", c => c.Int());
            AddColumn("dbo.ItemLoad", "CrmNo", c => c.String());
            DropColumn("dbo.ItemLoad", "CmrStatus");
            DropColumn("dbo.ItemLoad", "CmrNo");
        }
    }
}
