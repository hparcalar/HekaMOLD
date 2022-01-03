namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class machineUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Machine", "Width", c => c.Int());
            AddColumn("dbo.Machine", "NumberOfFramaes", c => c.String());
            AddColumn("dbo.Machine", "WeavingDraft", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Machine", "WeavingDraft");
            DropColumn("dbo.Machine", "NumberOfFramaes");
            DropColumn("dbo.Machine", "Width");
        }
    }
}
