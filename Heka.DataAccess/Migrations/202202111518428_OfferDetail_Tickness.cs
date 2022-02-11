namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OfferDetail_Tickness : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemOfferDetail", "SheetTickness", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemOfferDetail", "SheetTickness");
        }
    }
}
