namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageDetailUpdateX1Add : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VoyageDetail", "DeclarationX1No", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VoyageDetail", "DeclarationX1No");
        }
    }
}
