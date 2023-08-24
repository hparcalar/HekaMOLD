namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PreventiveActions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PreventiveAction",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicantName = c.String(),
                        ApplicantIdentity = c.String(),
                        ApplicantFirmName = c.String(),
                        ApplicantTitle = c.String(),
                        ActionType = c.String(),
                        FormDate = c.DateTime(),
                        FormNo = c.String(),
                        Declaration = c.String(),
                        RootCause = c.String(),
                        SolutionProposal = c.String(),
                        FormResult = c.Int(),
                        ApproverUserId = c.Int(),
                        ClosingUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.ApproverUserId)
                .ForeignKey("dbo.User", t => t.ClosingUserId)
                .Index(t => t.ApproverUserId)
                .Index(t => t.ClosingUserId);
            
            CreateTable(
                "dbo.PreventiveActionDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PreventiveActionId = c.Int(),
                        ResponsibleUserId = c.Int(),
                        ActionText = c.String(),
                        Notes = c.String(),
                        DeadlineDate = c.DateTime(),
                        ActionStatus = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.ResponsibleUserId)
                .ForeignKey("dbo.PreventiveAction", t => t.PreventiveActionId)
                .Index(t => t.PreventiveActionId)
                .Index(t => t.ResponsibleUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PreventiveActionDetail", "PreventiveActionId", "dbo.PreventiveAction");
            DropForeignKey("dbo.PreventiveActionDetail", "ResponsibleUserId", "dbo.User");
            DropForeignKey("dbo.PreventiveAction", "ClosingUserId", "dbo.User");
            DropForeignKey("dbo.PreventiveAction", "ApproverUserId", "dbo.User");
            DropIndex("dbo.PreventiveActionDetail", new[] { "ResponsibleUserId" });
            DropIndex("dbo.PreventiveActionDetail", new[] { "PreventiveActionId" });
            DropIndex("dbo.PreventiveAction", new[] { "ClosingUserId" });
            DropIndex("dbo.PreventiveAction", new[] { "ApproverUserId" });
            DropTable("dbo.PreventiveActionDetail");
            DropTable("dbo.PreventiveAction");
        }
    }
}
