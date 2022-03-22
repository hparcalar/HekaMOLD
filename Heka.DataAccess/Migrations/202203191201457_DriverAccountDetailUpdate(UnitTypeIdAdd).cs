namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DriverAccountDetailUpdateUnitTypeIdAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DriverAccountDetail", "CountryId", c => c.Int());
            AddColumn("dbo.DriverAccountDetail", "UnitTypeId", c => c.Int());
            AddColumn("dbo.DriverAccountDetail", "KmHour", c => c.Int());
            AddColumn("dbo.DriverAccountDetail", "Quantity", c => c.Int());
            AddColumn("dbo.DriverAccountDetail", "OperationDate", c => c.DateTime());
            CreateIndex("dbo.DriverAccountDetail", "CountryId");
            CreateIndex("dbo.DriverAccountDetail", "UnitTypeId");
            AddForeignKey("dbo.DriverAccountDetail", "UnitTypeId", "dbo.UnitType", "Id");
            AddForeignKey("dbo.DriverAccountDetail", "CountryId", "dbo.Country", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DriverAccountDetail", "CountryId", "dbo.Country");
            DropForeignKey("dbo.DriverAccountDetail", "UnitTypeId", "dbo.UnitType");
            DropIndex("dbo.DriverAccountDetail", new[] { "UnitTypeId" });
            DropIndex("dbo.DriverAccountDetail", new[] { "CountryId" });
            DropColumn("dbo.DriverAccountDetail", "OperationDate");
            DropColumn("dbo.DriverAccountDetail", "Quantity");
            DropColumn("dbo.DriverAccountDetail", "KmHour");
            DropColumn("dbo.DriverAccountDetail", "UnitTypeId");
            DropColumn("dbo.DriverAccountDetail", "CountryId");
        }
    }
}
