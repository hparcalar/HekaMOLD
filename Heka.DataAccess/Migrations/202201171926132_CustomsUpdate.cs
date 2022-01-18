namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomsUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemOrder", "ExitCustomsId", c => c.Int());
            CreateIndex("dbo.ItemOrder", "ExitCustomsId");
            AddForeignKey("dbo.ItemOrder", "ExitCustomsId", "dbo.Customs", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemOrder", "ExitCustomsId", "dbo.Customs");
            DropIndex("dbo.ItemOrder", new[] { "ExitCustomsId" });
            DropColumn("dbo.ItemOrder", "ExitCustomsId");
        }
    }
}
