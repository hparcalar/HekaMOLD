namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MaintenancePlan : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MaintenancePlan",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MachineId = c.Int(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        Subject = c.String(),
                        Explanation = c.String(),
                        ResultExplanation = c.String(),
                        ResponsibleId = c.Int(),
                        PlanStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Machine", t => t.MachineId)
                .ForeignKey("dbo.User", t => t.ResponsibleId)
                .Index(t => t.MachineId)
                .Index(t => t.ResponsibleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MaintenancePlan", "ResponsibleId", "dbo.User");
            DropForeignKey("dbo.MaintenancePlan", "MachineId", "dbo.Machine");
            DropIndex("dbo.MaintenancePlan", new[] { "ResponsibleId" });
            DropIndex("dbo.MaintenancePlan", new[] { "MachineId" });
            DropTable("dbo.MaintenancePlan");
        }
    }
}
