namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class YarnColourAndYarnColourGroup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.YarnBreed",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        YarnBreedCode = c.String(),
                        YarnBreedName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.YarnColour",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        YarnColourCode = c.String(),
                        YarnColourName = c.String(),
                        YarnColourGroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.YarnColourGroup", t => t.YarnColourGroupId, cascadeDelete: true)
                .Index(t => t.YarnColourGroupId);
            
            CreateTable(
                "dbo.YarnColourGroup",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        YarnColourGroupCode = c.String(),
                        YarnColourGroupName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.YarnRecipe", "YarnBreedId", c => c.Int(nullable: false));
            AddColumn("dbo.YarnRecipe", "Factor", c => c.Int());
            AddColumn("dbo.YarnRecipe", "Twist", c => c.Int());
            AddColumn("dbo.YarnRecipe", "Center", c => c.Boolean());
            AddColumn("dbo.YarnRecipe", "Mix", c => c.Boolean());
            AddColumn("dbo.YarnRecipe", "YarnColourId", c => c.Int(nullable: false));
            AddColumn("dbo.YarnRecipe", "YarnLot", c => c.Int(nullable: false));
            AddColumn("dbo.YarnRecipe", "StockCode", c => c.String());
            CreateIndex("dbo.YarnRecipe", "YarnBreedId");
            CreateIndex("dbo.YarnRecipe", "YarnColourId");
            AddForeignKey("dbo.YarnRecipe", "YarnBreedId", "dbo.YarnBreed", "Id", cascadeDelete: true);
            AddForeignKey("dbo.YarnRecipe", "YarnColourId", "dbo.YarnColour", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.YarnRecipe", "YarnColourId", "dbo.YarnColour");
            DropForeignKey("dbo.YarnColour", "YarnColourGroupId", "dbo.YarnColourGroup");
            DropForeignKey("dbo.YarnRecipe", "YarnBreedId", "dbo.YarnBreed");
            DropIndex("dbo.YarnColour", new[] { "YarnColourGroupId" });
            DropIndex("dbo.YarnRecipe", new[] { "YarnColourId" });
            DropIndex("dbo.YarnRecipe", new[] { "YarnBreedId" });
            DropColumn("dbo.YarnRecipe", "StockCode");
            DropColumn("dbo.YarnRecipe", "YarnLot");
            DropColumn("dbo.YarnRecipe", "YarnColourId");
            DropColumn("dbo.YarnRecipe", "Mix");
            DropColumn("dbo.YarnRecipe", "Center");
            DropColumn("dbo.YarnRecipe", "Twist");
            DropColumn("dbo.YarnRecipe", "Factor");
            DropColumn("dbo.YarnRecipe", "YarnBreedId");
            DropTable("dbo.YarnColourGroup");
            DropTable("dbo.YarnColour");
            DropTable("dbo.YarnBreed");
        }
    }
}
