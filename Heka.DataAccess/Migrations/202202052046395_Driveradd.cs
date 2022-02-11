namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Driveradd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Driver",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DriverName = c.String(),
                        DriverSurName = c.String(),
                        Tc = c.String(),
                        PlaceOfBirth = c.String(),
                        BirthDate = c.DateTime(nullable: false),
                        PassportNo = c.String(),
                        CountryId = c.Int(nullable: false),
                        VisaType = c.Int(),
                        VisaStartDate = c.DateTime(nullable: false),
                        VisaEndDate = c.DateTime(nullable: false),
                        ProfileImage = c.Binary(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Country", t => t.CountryId, cascadeDelete: true)
                .Index(t => t.CountryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Driver", "CountryId", "dbo.Country");
            DropIndex("dbo.Driver", new[] { "CountryId" });
            DropTable("dbo.Driver");
        }
    }
}
