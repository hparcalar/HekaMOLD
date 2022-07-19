namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemReceipt_Driver : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemReceipt", "Driver", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemReceipt", "Driver");
        }
    }
}
