namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SheetProgramName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemOrder", "SheetProgramName", c => c.String());
            AddColumn("dbo.ItemOffer", "SheetProgramName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemOffer", "SheetProgramName");
            DropColumn("dbo.ItemOrder", "SheetProgramName");
        }
    }
}
