namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateCustomsArrivalAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "CustomsArrivalId", c => c.Int());
            CreateIndex("dbo.ItemLoad", "CustomsArrivalId");
            AddForeignKey("dbo.ItemLoad", "CustomsArrivalId", "dbo.Customs", "Id");
            DropColumn("dbo.ItemLoad", "ArrivalCustoms");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemLoad", "ArrivalCustoms", c => c.String());
            DropForeignKey("dbo.ItemLoad", "CustomsArrivalId", "dbo.Customs");
            DropIndex("dbo.ItemLoad", new[] { "CustomsArrivalId" });
            DropColumn("dbo.ItemLoad", "CustomsArrivalId");
        }
    }
}
