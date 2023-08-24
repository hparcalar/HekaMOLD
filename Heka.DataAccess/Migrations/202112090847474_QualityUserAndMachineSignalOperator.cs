namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QualityUserAndMachineSignalOperator : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MachineSignal", "OperatorId", c => c.Int());
            AddColumn("dbo.WorkOrderSerial", "QualityChangedDate", c => c.DateTime());
            AddColumn("dbo.WorkOrderSerial", "QualityUserId", c => c.Int());
            CreateIndex("dbo.MachineSignal", "OperatorId");
            CreateIndex("dbo.WorkOrderSerial", "QualityUserId");
            AddForeignKey("dbo.MachineSignal", "OperatorId", "dbo.User", "Id");
            AddForeignKey("dbo.WorkOrderSerial", "QualityUserId", "dbo.User", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WorkOrderSerial", "QualityUserId", "dbo.User");
            DropForeignKey("dbo.MachineSignal", "OperatorId", "dbo.User");
            DropIndex("dbo.WorkOrderSerial", new[] { "QualityUserId" });
            DropIndex("dbo.MachineSignal", new[] { "OperatorId" });
            DropColumn("dbo.WorkOrderSerial", "QualityUserId");
            DropColumn("dbo.WorkOrderSerial", "QualityChangedDate");
            DropColumn("dbo.MachineSignal", "OperatorId");
        }
    }
}
