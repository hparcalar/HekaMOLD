namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemOrderAddedOrderUploadPointType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemOrder", "OrderUploadPointType", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemOrder", "OrderUploadPointType");
        }
    }
}
