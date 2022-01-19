namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemOrderandItemOrderDetailUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemOrderDetail", "OrderProductBreed", c => c.Int());
            AddColumn("dbo.ItemOrderDetail", "PackageInNumber", c => c.Int());
            AddColumn("dbo.ItemOrder", "OrderCalculationType", c => c.Int());
            AddColumn("dbo.ItemOrder", "LoadOutDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemOrder", "LoadOutDate");
            DropColumn("dbo.ItemOrder", "OrderCalculationType");
            DropColumn("dbo.ItemOrderDetail", "PackageInNumber");
            DropColumn("dbo.ItemOrderDetail", "OrderProductBreed");
        }
    }
}
