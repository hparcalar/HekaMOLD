namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OfferParamsForHeader : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemOffer", "SheetWeight", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemOffer", "LaborCost", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemOffer", "WastageWeight", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemOffer", "ProfitRate", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemOffer", "CreditMonths", c => c.Int());
            AddColumn("dbo.ItemOffer", "CreditRate", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemOffer", "CreditRate");
            DropColumn("dbo.ItemOffer", "CreditMonths");
            DropColumn("dbo.ItemOffer", "ProfitRate");
            DropColumn("dbo.ItemOffer", "WastageWeight");
            DropColumn("dbo.ItemOffer", "LaborCost");
            DropColumn("dbo.ItemOffer", "SheetWeight");
        }
    }
}
