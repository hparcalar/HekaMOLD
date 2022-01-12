namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MachineWeavingDraft : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Machine", "WeavingDraftInfo", c => c.String());
            AddColumn("dbo.Machine", "WeavingDraftId", c => c.Int());
            CreateIndex("dbo.Machine", "WeavingDraftId");
            AddForeignKey("dbo.Machine", "WeavingDraftId", "dbo.WeavingDraft", "Id");
            DropColumn("dbo.Machine", "WeavingDraft");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Machine", "WeavingDraft", c => c.String());
            DropForeignKey("dbo.Machine", "WeavingDraftId", "dbo.WeavingDraft");
            DropIndex("dbo.Machine", new[] { "WeavingDraftId" });
            DropColumn("dbo.Machine", "WeavingDraftId");
            DropColumn("dbo.Machine", "WeavingDraftInfo");
        }
    }
}
