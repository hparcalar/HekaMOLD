namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageDetailUpdateLoadingLineNoAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VoyageDetail", "LoadingLineNo", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VoyageDetail", "LoadingLineNo");
        }
    }
}
