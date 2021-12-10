namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateItem002 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Item", "MachineId", "dbo.Machine");
            DropIndex("dbo.Item", new[] { "MachineId" });
            AlterColumn("dbo.Item", "MachineId", c => c.Int());
            CreateIndex("dbo.Item", "MachineId");
            AddForeignKey("dbo.Item", "MachineId", "dbo.Machine", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Item", "MachineId", "dbo.Machine");
            DropIndex("dbo.Item", new[] { "MachineId" });
            AlterColumn("dbo.Item", "MachineId", c => c.Int(nullable: false));
            CreateIndex("dbo.Item", "MachineId");
            AddForeignKey("dbo.Item", "MachineId", "dbo.Machine", "Id", cascadeDelete: true);
        }
    }
}
