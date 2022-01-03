namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SerialQualityWinding : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SerialQualityWinding",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SerialNo = c.String(),
                        WorkOrderDetailId = c.Int(),
                        WorkOrderSerialId = c.Int(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        OperatorId = c.Int(),
                        IsOk = c.Boolean(),
                        FaultCount = c.Int(),
                        Explanation = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.OperatorId)
                .ForeignKey("dbo.WorkOrderDetail", t => t.WorkOrderDetailId)
                .ForeignKey("dbo.WorkOrderSerial", t => t.WorkOrderSerialId)
                .Index(t => t.WorkOrderDetailId)
                .Index(t => t.WorkOrderSerialId)
                .Index(t => t.OperatorId);
            
            CreateTable(
                "dbo.SerialQualityWindingFault",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SerialQualityWindingId = c.Int(),
                        FaultId = c.Int(),
                        CurrentMeter = c.Decimal(precision: 18, scale: 2),
                        CurrentQuantity = c.Decimal(precision: 18, scale: 2),
                        Explanation = c.String(),
                        OperatorId = c.Int(),
                        FaultDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SerialFaultType", t => t.FaultId)
                .ForeignKey("dbo.User", t => t.OperatorId)
                .ForeignKey("dbo.SerialQualityWinding", t => t.SerialQualityWindingId)
                .Index(t => t.SerialQualityWindingId)
                .Index(t => t.FaultId)
                .Index(t => t.OperatorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SerialQualityWinding", "WorkOrderSerialId", "dbo.WorkOrderSerial");
            DropForeignKey("dbo.SerialQualityWinding", "WorkOrderDetailId", "dbo.WorkOrderDetail");
            DropForeignKey("dbo.SerialQualityWinding", "OperatorId", "dbo.User");
            DropForeignKey("dbo.SerialQualityWindingFault", "SerialQualityWindingId", "dbo.SerialQualityWinding");
            DropForeignKey("dbo.SerialQualityWindingFault", "OperatorId", "dbo.User");
            DropForeignKey("dbo.SerialQualityWindingFault", "FaultId", "dbo.SerialFaultType");
            DropIndex("dbo.SerialQualityWindingFault", new[] { "OperatorId" });
            DropIndex("dbo.SerialQualityWindingFault", new[] { "FaultId" });
            DropIndex("dbo.SerialQualityWindingFault", new[] { "SerialQualityWindingId" });
            DropIndex("dbo.SerialQualityWinding", new[] { "OperatorId" });
            DropIndex("dbo.SerialQualityWinding", new[] { "WorkOrderSerialId" });
            DropIndex("dbo.SerialQualityWinding", new[] { "WorkOrderDetailId" });
            DropTable("dbo.SerialQualityWindingFault");
            DropTable("dbo.SerialQualityWinding");
        }
    }
}
