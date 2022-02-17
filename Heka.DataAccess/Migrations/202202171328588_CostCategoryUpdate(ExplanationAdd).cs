namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CostCategoryUpdateExplanationAdd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CostCategory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CostCategoryCode = c.String(),
                        CostCategoryName = c.String(),
                        Explanation = c.String(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CostCategory");
        }
    }
}
