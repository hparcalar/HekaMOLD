namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class YarnRecipeUpdateSalesPriceAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.YarnRecipe", "BuyingPrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.YarnRecipe", "SalesPrice", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.YarnRecipe", "Price");
        }
        
        public override void Down()
        {
            AddColumn("dbo.YarnRecipe", "Price", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.YarnRecipe", "SalesPrice");
            DropColumn("dbo.YarnRecipe", "BuyingPrice");
        }
    }
}
