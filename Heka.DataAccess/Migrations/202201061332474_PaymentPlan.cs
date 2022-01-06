namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PaymentPlan : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PaymentPlan",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PaymentPlanCode = c.String(),
                        PaymentPlanName = c.String(),
                        IntegrationCode = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.ItemOrder", "PaymentPlanId", c => c.Int());
            CreateIndex("dbo.ItemOrder", "PaymentPlanId");
            AddForeignKey("dbo.ItemOrder", "PaymentPlanId", "dbo.PaymentPlan", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemOrder", "PaymentPlanId", "dbo.PaymentPlan");
            DropIndex("dbo.ItemOrder", new[] { "PaymentPlanId" });
            DropColumn("dbo.ItemOrder", "PaymentPlanId");
            DropTable("dbo.PaymentPlan");
        }
    }
}
