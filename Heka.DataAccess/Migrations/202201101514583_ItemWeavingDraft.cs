namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemWeavingDraft : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Item", "MachineId", "dbo.Machine");
            DropIndex("dbo.Item", new[] { "MachineId" });
            DropColumn("dbo.Item", "MachineId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Item", "MachineId", c => c.Int());
            CreateIndex("dbo.Item", "MachineId");
            AddForeignKey("dbo.Item", "MachineId", "dbo.Machine", "Id");
        }
    }
}
