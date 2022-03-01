namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirmAddressAdd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FirmAddress",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddressName = c.String(),
                        FirmId = c.Int(nullable: false),
                        CityId = c.Int(),
                        CountryId = c.Int(),
                        Address1 = c.String(),
                        Address2 = c.String(),
                        MobilePhone = c.String(),
                        Fax = c.String(),
                        OfficePhone = c.String(),
                        AuthorizedInfo = c.String(),
                        AddressType = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Country", t => t.CountryId)
                .ForeignKey("dbo.City", t => t.CityId)
                .ForeignKey("dbo.Firm", t => t.FirmId, cascadeDelete: true)
                .Index(t => t.FirmId)
                .Index(t => t.CityId)
                .Index(t => t.CountryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FirmAddress", "FirmId", "dbo.Firm");
            DropForeignKey("dbo.FirmAddress", "CityId", "dbo.City");
            DropForeignKey("dbo.FirmAddress", "CountryId", "dbo.Country");
            DropIndex("dbo.FirmAddress", new[] { "CountryId" });
            DropIndex("dbo.FirmAddress", new[] { "CityId" });
            DropIndex("dbo.FirmAddress", new[] { "FirmId" });
            DropTable("dbo.FirmAddress");
        }
    }
}
