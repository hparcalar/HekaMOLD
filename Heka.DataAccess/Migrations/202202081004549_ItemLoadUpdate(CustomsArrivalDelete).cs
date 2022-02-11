namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateCustomsArrivalDelete : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ItemLoad", "CustomsArrivalId", "dbo.Customs");
            DropIndex("dbo.ItemLoad", new[] { "CustomsArrivalId" });
            DropColumn("dbo.ItemLoad", "CustomsArrivalId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemLoad", "CustomsArrivalId", c => c.Int());
            CreateIndex("dbo.ItemLoad", "CustomsArrivalId");
            AddForeignKey("dbo.ItemLoad", "CustomsArrivalId", "dbo.Customs", "Id");
        }
    }
}
