namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class YarnColourGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.YarnRecipe", "PlantId", c => c.Int());
            AddColumn("dbo.YarnRecipe", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.YarnRecipe", "CreatedUserId", c => c.Int());
            AddColumn("dbo.YarnRecipe", "UpdatedDate", c => c.DateTime());
            AddColumn("dbo.YarnRecipe", "UpdatedUserId", c => c.Int());
            AddColumn("dbo.YarnColour", "PlantId", c => c.Int());
            AddColumn("dbo.YarnColour", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.YarnColour", "CreatedUserId", c => c.Int());
            AddColumn("dbo.YarnColour", "UpdatedDate", c => c.DateTime());
            AddColumn("dbo.YarnColour", "UpdatedUserId", c => c.Int());
            AddColumn("dbo.YarnColourGroup", "PlantId", c => c.Int());
            AddColumn("dbo.YarnColourGroup", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.YarnColourGroup", "CreatedUserId", c => c.Int());
            AddColumn("dbo.YarnColourGroup", "UpdatedDate", c => c.DateTime());
            AddColumn("dbo.YarnColourGroup", "UpdatedUserId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.YarnColourGroup", "UpdatedUserId");
            DropColumn("dbo.YarnColourGroup", "UpdatedDate");
            DropColumn("dbo.YarnColourGroup", "CreatedUserId");
            DropColumn("dbo.YarnColourGroup", "CreatedDate");
            DropColumn("dbo.YarnColourGroup", "PlantId");
            DropColumn("dbo.YarnColour", "UpdatedUserId");
            DropColumn("dbo.YarnColour", "UpdatedDate");
            DropColumn("dbo.YarnColour", "CreatedUserId");
            DropColumn("dbo.YarnColour", "CreatedDate");
            DropColumn("dbo.YarnColour", "PlantId");
            DropColumn("dbo.YarnRecipe", "UpdatedUserId");
            DropColumn("dbo.YarnRecipe", "UpdatedDate");
            DropColumn("dbo.YarnRecipe", "CreatedUserId");
            DropColumn("dbo.YarnRecipe", "CreatedDate");
            DropColumn("dbo.YarnRecipe", "PlantId");
        }
    }
}
