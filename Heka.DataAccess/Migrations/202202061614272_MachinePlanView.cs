namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MachinePlanView : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MachinePlanView",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ViewDate = c.DateTime(),
                        PlantId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.PlantId);
            
            CreateTable(
                "dbo.MachinePlanViewDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MachinePlanViewId = c.Int(),
                        MachineId = c.Int(),
                        WorkOrderDetailId = c.Int(),
                        OrderNo = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Machine", t => t.MachineId)
                .ForeignKey("dbo.MachinePlanView", t => t.MachinePlanViewId)
                .ForeignKey("dbo.WorkOrderDetail", t => t.WorkOrderDetailId)
                .Index(t => t.MachinePlanViewId)
                .Index(t => t.MachineId)
                .Index(t => t.WorkOrderDetailId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MachinePlanViewDetail", "WorkOrderDetailId", "dbo.WorkOrderDetail");
            DropForeignKey("dbo.MachinePlanViewDetail", "MachinePlanViewId", "dbo.MachinePlanView");
            DropForeignKey("dbo.MachinePlanViewDetail", "MachineId", "dbo.Machine");
            DropForeignKey("dbo.MachinePlanView", "PlantId", "dbo.Plant");
            DropIndex("dbo.MachinePlanViewDetail", new[] { "WorkOrderDetailId" });
            DropIndex("dbo.MachinePlanViewDetail", new[] { "MachineId" });
            DropIndex("dbo.MachinePlanViewDetail", new[] { "MachinePlanViewId" });
            DropIndex("dbo.MachinePlanView", new[] { "PlantId" });
            DropTable("dbo.MachinePlanViewDetail");
            DropTable("dbo.MachinePlanView");
        }
    }
}
