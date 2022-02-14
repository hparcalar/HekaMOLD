namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoundCostCategoryAdd1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LoadCostCategory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LoadCostCategoryCode = c.String(),
                        LoadCostCategoryName = c.String(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.LoadCostCategory");
        }
    }
}
