namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirmTariffUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FirmTariff", "LadametrePrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.FirmTariff", "MeetrCupPrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.FirmTariff", "WeightPrice", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.FirmTariff", "Ladametre");
            DropColumn("dbo.FirmTariff", "MeetrCup");
            DropColumn("dbo.FirmTariff", "Weight");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FirmTariff", "Weight", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.FirmTariff", "MeetrCup", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.FirmTariff", "Ladametre", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.FirmTariff", "WeightPrice");
            DropColumn("dbo.FirmTariff", "MeetrCupPrice");
            DropColumn("dbo.FirmTariff", "LadametrePrice");
        }
    }
}
