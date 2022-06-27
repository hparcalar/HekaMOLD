namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WorkOrderSerial_ShiftBelongsToDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WorkOrderSerial", "ShiftBelongsToDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.WorkOrderSerial", "ShiftBelongsToDate");
        }
    }
}
