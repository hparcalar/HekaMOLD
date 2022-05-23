namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomerComplaints : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerComplaint",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FormNo = c.String(),
                        FormDate = c.DateTime(),
                        IncomingType = c.Int(),
                        FirmId = c.Int(),
                        CustomerDocumentNo = c.String(),
                        Explanation = c.String(),
                        Notes = c.String(),
                        FormStatus = c.Int(),
                        ActionDate = c.DateTime(),
                        ClosedDate = c.DateTime(),
                        PreventiveActionId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Firm", t => t.FirmId)
                .ForeignKey("dbo.PreventiveAction", t => t.PreventiveActionId)
                .Index(t => t.FirmId)
                .Index(t => t.PreventiveActionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerComplaint", "PreventiveActionId", "dbo.PreventiveAction");
            DropForeignKey("dbo.CustomerComplaint", "FirmId", "dbo.Firm");
            DropIndex("dbo.CustomerComplaint", new[] { "PreventiveActionId" });
            DropIndex("dbo.CustomerComplaint", new[] { "FirmId" });
            DropTable("dbo.CustomerComplaint");
        }
    }
}
