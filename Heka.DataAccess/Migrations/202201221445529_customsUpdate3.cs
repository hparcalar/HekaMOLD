namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class customsUpdate3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customs", "CityId", c => c.Int());
            CreateIndex("dbo.Customs", "CityId");
            AddForeignKey("dbo.Customs", "CityId", "dbo.City", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Customs", "CityId", "dbo.City");
            DropIndex("dbo.Customs", new[] { "CityId" });
            DropColumn("dbo.Customs", "CityId");
        }
    }
}
