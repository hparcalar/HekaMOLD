namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemSerial_ShiftBelongsToDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemSerial", "ShiftBelongsToDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemSerial", "ShiftBelongsToDate");
        }
    }
}
