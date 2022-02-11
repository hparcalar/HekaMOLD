namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Driverupdate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Driver", "CountryId", "dbo.Country");
            DropIndex("dbo.Driver", new[] { "CountryId" });
            AlterColumn("dbo.Driver", "BirthDate", c => c.DateTime());
            AlterColumn("dbo.Driver", "CountryId", c => c.Int());
            AlterColumn("dbo.Driver", "VisaStartDate", c => c.DateTime());
            AlterColumn("dbo.Driver", "VisaEndDate", c => c.DateTime());
            CreateIndex("dbo.Driver", "CountryId");
            AddForeignKey("dbo.Driver", "CountryId", "dbo.Country", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Driver", "CountryId", "dbo.Country");
            DropIndex("dbo.Driver", new[] { "CountryId" });
            AlterColumn("dbo.Driver", "VisaEndDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Driver", "VisaStartDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Driver", "CountryId", c => c.Int(nullable: false));
            AlterColumn("dbo.Driver", "BirthDate", c => c.DateTime(nullable: false));
            CreateIndex("dbo.Driver", "CountryId");
            AddForeignKey("dbo.Driver", "CountryId", "dbo.Country", "Id", cascadeDelete: true);
        }
    }
}
