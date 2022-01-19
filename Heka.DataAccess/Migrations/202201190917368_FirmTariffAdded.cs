namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirmTariffAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FirmTariff",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Ladametre = c.Decimal(precision: 18, scale: 2),
                        MeetrCup = c.Decimal(precision: 18, scale: 2),
                        Weight = c.Decimal(precision: 18, scale: 2),
                        ForexTypeId = c.Int(),
                        FirmId = c.Int(nullable: false),
                        UnitTypeId = c.Int(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                        IsActive = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UnitType", t => t.UnitTypeId)
                .ForeignKey("dbo.ForexType", t => t.ForexTypeId)
                .ForeignKey("dbo.Firm", t => t.FirmId, cascadeDelete: true)
                .Index(t => t.ForexTypeId)
                .Index(t => t.FirmId)
                .Index(t => t.UnitTypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FirmTariff", "FirmId", "dbo.Firm");
            DropForeignKey("dbo.FirmTariff", "ForexTypeId", "dbo.ForexType");
            DropForeignKey("dbo.FirmTariff", "UnitTypeId", "dbo.UnitType");
            DropIndex("dbo.FirmTariff", new[] { "UnitTypeId" });
            DropIndex("dbo.FirmTariff", new[] { "FirmId" });
            DropIndex("dbo.FirmTariff", new[] { "ForexTypeId" });
            DropTable("dbo.FirmTariff");
        }
    }
}
