namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoundCostCategoryDelete : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.RoundCostCategory");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.RoundCostCategory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoundCostCategoryCode = c.String(),
                        RoundCostCategoryName = c.String(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
