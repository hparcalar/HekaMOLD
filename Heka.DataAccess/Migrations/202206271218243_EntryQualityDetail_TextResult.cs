namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EntryQualityDetail_TextResult : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EntryQualityDataDetail", "TextResult", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.EntryQualityDataDetail", "TextResult");
        }
    }
}
