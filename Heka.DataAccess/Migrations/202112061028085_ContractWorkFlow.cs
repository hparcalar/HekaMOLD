namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ContractWorkFlow : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ContractWorkFlow",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WorkOrderDetailId = c.Int(),
                        DeliveredDetailId = c.Int(),
                        ReceivedDetailId = c.Int(),
                        FlowDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ItemReceiptDetail", t => t.DeliveredDetailId)
                .ForeignKey("dbo.ItemReceiptDetail", t => t.ReceivedDetailId)
                .ForeignKey("dbo.WorkOrderDetail", t => t.WorkOrderDetailId)
                .Index(t => t.WorkOrderDetailId)
                .Index(t => t.DeliveredDetailId)
                .Index(t => t.ReceivedDetailId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ContractWorkFlow", "WorkOrderDetailId", "dbo.WorkOrderDetail");
            DropForeignKey("dbo.ContractWorkFlow", "ReceivedDetailId", "dbo.ItemReceiptDetail");
            DropForeignKey("dbo.ContractWorkFlow", "DeliveredDetailId", "dbo.ItemReceiptDetail");
            DropIndex("dbo.ContractWorkFlow", new[] { "ReceivedDetailId" });
            DropIndex("dbo.ContractWorkFlow", new[] { "DeliveredDetailId" });
            DropIndex("dbo.ContractWorkFlow", new[] { "WorkOrderDetailId" });
            DropTable("dbo.ContractWorkFlow");
        }
    }
}
