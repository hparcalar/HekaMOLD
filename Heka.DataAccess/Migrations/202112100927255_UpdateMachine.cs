namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMachine : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Item", "Pattern", c => c.Int());
            AddColumn("dbo.Item", "CrudeWidth", c => c.Int());
            AddColumn("dbo.Item", "CrudeGramaj", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Item", "ProductWidth", c => c.Int());
            AddColumn("dbo.Item", "ProductGramaj", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Item", "WarpWireCount", c => c.Int());
            AddColumn("dbo.Item", "MeterGramaj", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Item", "Cutting", c => c.String());
            AddColumn("dbo.Item", "Dyehouse", c => c.String());
            AddColumn("dbo.Item", "Apparel", c => c.String());
            AddColumn("dbo.Item", "Bullet", c => c.String());
            AddColumn("dbo.Item", "CombWidth", c => c.Int(nullable: false));
            AddColumn("dbo.Item", "WeftReportLength", c => c.Int(nullable: false));
            AddColumn("dbo.Item", "WarpReportLength", c => c.Int(nullable: false));
            AddColumn("dbo.Item", "WeftDensity", c => c.Int(nullable: false));
            AddColumn("dbo.Item", "MachineId", c => c.Int(nullable: false));
            CreateIndex("dbo.Item", "MachineId");
            AddForeignKey("dbo.Item", "MachineId", "dbo.Machine", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Item", "MachineId", "dbo.Machine");
            DropIndex("dbo.Item", new[] { "MachineId" });
            DropColumn("dbo.Item", "MachineId");
            DropColumn("dbo.Item", "WeftDensity");
            DropColumn("dbo.Item", "WarpReportLength");
            DropColumn("dbo.Item", "WeftReportLength");
            DropColumn("dbo.Item", "CombWidth");
            DropColumn("dbo.Item", "Bullet");
            DropColumn("dbo.Item", "Apparel");
            DropColumn("dbo.Item", "Dyehouse");
            DropColumn("dbo.Item", "Cutting");
            DropColumn("dbo.Item", "MeterGramaj");
            DropColumn("dbo.Item", "WarpWireCount");
            DropColumn("dbo.Item", "ProductGramaj");
            DropColumn("dbo.Item", "ProductWidth");
            DropColumn("dbo.Item", "CrudeGramaj");
            DropColumn("dbo.Item", "CrudeWidth");
            DropColumn("dbo.Item", "Pattern");
        }
    }
}
