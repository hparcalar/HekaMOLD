namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Machine_PreProcess : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MachinePreProcess",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MachineId = c.Int(),
                        PreProcessTypeId = c.Int(),
                        LineNumber = c.Int(),
                        IsRequired = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PreProcessType", t => t.PreProcessTypeId)
                .ForeignKey("dbo.Machine", t => t.MachineId)
                .Index(t => t.MachineId)
                .Index(t => t.PreProcessTypeId);
            
            CreateTable(
                "dbo.PreProcessType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PreProcessCode = c.String(),
                        PreProcessName = c.String(),
                        HasMoldSelection = c.Boolean(),
                        TwoStepProcess = c.Boolean(),
                        PlantId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.PlantId);
            
            CreateTable(
                "dbo.MachinePreProcessHistory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MachineId = c.Int(),
                        MoldId = c.Int(),
                        CreatedUserId = c.Int(),
                        PreProcessTypeId = c.Int(),
                        WorkOrderDetailId = c.Int(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        Duration = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Machine", t => t.MachineId)
                .ForeignKey("dbo.Mold", t => t.MoldId)
                .ForeignKey("dbo.PreProcessType", t => t.PreProcessTypeId)
                .ForeignKey("dbo.User", t => t.CreatedUserId)
                .ForeignKey("dbo.WorkOrderDetail", t => t.WorkOrderDetailId)
                .Index(t => t.MachineId)
                .Index(t => t.MoldId)
                .Index(t => t.CreatedUserId)
                .Index(t => t.PreProcessTypeId)
                .Index(t => t.WorkOrderDetailId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MachinePreProcessHistory", "WorkOrderDetailId", "dbo.WorkOrderDetail");
            DropForeignKey("dbo.MachinePreProcessHistory", "CreatedUserId", "dbo.User");
            DropForeignKey("dbo.MachinePreProcessHistory", "PreProcessTypeId", "dbo.PreProcessType");
            DropForeignKey("dbo.MachinePreProcessHistory", "MoldId", "dbo.Mold");
            DropForeignKey("dbo.MachinePreProcessHistory", "MachineId", "dbo.Machine");
            DropForeignKey("dbo.MachinePreProcess", "MachineId", "dbo.Machine");
            DropForeignKey("dbo.MachinePreProcess", "PreProcessTypeId", "dbo.PreProcessType");
            DropForeignKey("dbo.PreProcessType", "PlantId", "dbo.Plant");
            DropIndex("dbo.MachinePreProcessHistory", new[] { "WorkOrderDetailId" });
            DropIndex("dbo.MachinePreProcessHistory", new[] { "PreProcessTypeId" });
            DropIndex("dbo.MachinePreProcessHistory", new[] { "CreatedUserId" });
            DropIndex("dbo.MachinePreProcessHistory", new[] { "MoldId" });
            DropIndex("dbo.MachinePreProcessHistory", new[] { "MachineId" });
            DropIndex("dbo.PreProcessType", new[] { "PlantId" });
            DropIndex("dbo.MachinePreProcess", new[] { "PreProcessTypeId" });
            DropIndex("dbo.MachinePreProcess", new[] { "MachineId" });
            DropTable("dbo.MachinePreProcessHistory");
            DropTable("dbo.PreProcessType");
            DropTable("dbo.MachinePreProcess");
        }
    }
}
