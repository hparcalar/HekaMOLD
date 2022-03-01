namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageUpdateDriverSubsistenceAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Voyage", "DriverSubsistence", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Voyage", "ForexTypeId", c => c.Int());
            CreateIndex("dbo.Voyage", "ForexTypeId");
            AddForeignKey("dbo.Voyage", "ForexTypeId", "dbo.ForexType", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Voyage", "ForexTypeId", "dbo.ForexType");
            DropIndex("dbo.Voyage", new[] { "ForexTypeId" });
            DropColumn("dbo.Voyage", "ForexTypeId");
            DropColumn("dbo.Voyage", "DriverSubsistence");
        }
    }
}
