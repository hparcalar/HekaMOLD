namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemOrderNeedsSummary : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemOrderItemNeeds", "RecipeQuantityInKg", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemOrderItemNeeds", "ItemText", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemOrderItemNeeds", "ItemText");
            DropColumn("dbo.ItemOrderItemNeeds", "RecipeQuantityInKg");
        }
    }
}
