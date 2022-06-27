namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoldRevisions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MoldRevisionOperation",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemReceiptDetailId = c.Int(),
                        MoldId = c.Int(),
                        Explanation = c.String(),
                        OperationStatus = c.Int(),
                        OperationResult = c.Int(),
                        FinishDate = c.DateTime(),
                        ResultExplanation = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ItemReceiptDetail", t => t.ItemReceiptDetailId)
                .ForeignKey("dbo.Mold", t => t.MoldId)
                .Index(t => t.ItemReceiptDetailId)
                .Index(t => t.MoldId);
            
            AddColumn("dbo.Mold", "InWarehouseId", c => c.Int());
            CreateIndex("dbo.Mold", "InWarehouseId");
            AddForeignKey("dbo.Mold", "InWarehouseId", "dbo.Warehouse", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MoldRevisionOperation", "MoldId", "dbo.Mold");
            DropForeignKey("dbo.MoldRevisionOperation", "ItemReceiptDetailId", "dbo.ItemReceiptDetail");
            DropForeignKey("dbo.Mold", "InWarehouseId", "dbo.Warehouse");
            DropIndex("dbo.MoldRevisionOperation", new[] { "MoldId" });
            DropIndex("dbo.MoldRevisionOperation", new[] { "ItemReceiptDetailId" });
            DropIndex("dbo.Mold", new[] { "InWarehouseId" });
            DropColumn("dbo.Mold", "InWarehouseId");
            DropTable("dbo.MoldRevisionOperation");
        }
    }
}
