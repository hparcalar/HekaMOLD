namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageUpdateLoadCustomsAdd : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Voyage", "LoadCustomsId");
            CreateIndex("dbo.Voyage", "DischargeCustomsId");
            AddForeignKey("dbo.Voyage", "DischargeCustomsId", "dbo.Customs", "Id");
            AddForeignKey("dbo.Voyage", "LoadCustomsId", "dbo.Customs", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Voyage", "LoadCustomsId", "dbo.Customs");
            DropForeignKey("dbo.Voyage", "DischargeCustomsId", "dbo.Customs");
            DropIndex("dbo.Voyage", new[] { "DischargeCustomsId" });
            DropIndex("dbo.Voyage", new[] { "LoadCustomsId" });
        }
    }
}
