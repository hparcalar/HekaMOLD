namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateFirmAgentAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "AgentFirmId", c => c.Int());
            CreateIndex("dbo.ItemLoad", "AgentFirmId");
            AddForeignKey("dbo.ItemLoad", "AgentFirmId", "dbo.Firm", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemLoad", "AgentFirmId", "dbo.Firm");
            DropIndex("dbo.ItemLoad", new[] { "AgentFirmId" });
            DropColumn("dbo.ItemLoad", "AgentFirmId");
        }
    }
}
