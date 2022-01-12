namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OfferExpiration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemOffer", "Expiration", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemOffer", "Expiration");
        }
    }
}
